using Microsoft.AspNetCore.SignalR;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.DTOs.GameStream;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;
using TheBackfield.Utilities;

namespace TheBackfield.Services
{
    public class GameStreamService : IGameStreamService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IPlayRepository _playRepository;
        private readonly IHubContext<WatchGame, IWatchClient> _watchContext;

        public GameStreamService(
            IGameRepository gameRepository,
            IPlayRepository playRepository,
            IHubContext<WatchGame, IWatchClient> watchContext
            )
        {
            _gameRepository = gameRepository;
            _playRepository = playRepository;
            _watchContext = watchContext;
        }
        public async Task<GameStreamDTO?> GetGameStreamAsync(int gameId)
        {
            Game? game = await _gameRepository.GetSingleGameAllStatsAsync(gameId);
            if (game == null)
            {
                return null;
            }

            int down = 0;
            int? toGain = null;
            int? fieldPositionStart = null;
            int? nextTeamId = null;
            PlaySubmitWithSegmentsDTO? lastPlay = null;

            int currentPlayId = game.Plays.SingleOrDefault(p => !game.Plays.Any(gp => gp.PrevPlayId == p.Id))?.Id ?? 0;
            Play? currentPlay = await _playRepository.GetSinglePlayAsync(currentPlayId);
            var (homeTeamScore, awayTeamScore) = StatClient.ParseScore(game);

            if (currentPlay != null)
            {
                (down, toGain, fieldPositionStart, nextTeamId) = StatClient.ParseNextFieldPosition(currentPlay, game.HomeTeamId, game.AwayTeamId);
                PlaySubmitDTO currentPlayAsDTO = PlayClient.PlayAsPlaySubmitDTO(currentPlay);
                lastPlay = new PlaySubmitWithSegmentsDTO(currentPlayAsDTO, await GetPlaySegmentsAsync(currentPlayId));
            }

            int? clockStart = null;
            int? gamePeriod = null;
            int playCheckId = currentPlay?.Id ?? 0;
            do
            {
                Play? checkPlay = game.Plays.SingleOrDefault(p => p.Id == playCheckId);
                if (checkPlay == null)
                {
                    clockStart = game.PeriodLength;
                    gamePeriod = 1;
                }
                else
                {
                    if (gamePeriod == null && checkPlay.GamePeriod != null)
                    {
                        gamePeriod = checkPlay.GamePeriod;
                    }
                    if (checkPlay.ClockEnd != null)
                    {
                        clockStart = checkPlay.ClockEnd;
                    }
                    else if (checkPlay.ClockStart != null)
                    {
                        clockStart = checkPlay.ClockStart;
                    }
                    else if (gamePeriod != null && (checkPlay.GamePeriod ?? gamePeriod) != gamePeriod)
                    {
                        clockStart = game.PeriodLength;
                    }
                    else
                    {
                        playCheckId = checkPlay.PrevPlayId ?? 0;
                    }
                }
            } while (clockStart == null || gamePeriod == null);

            if (clockStart == 0)
            {
                gamePeriod += 1;
                clockStart = game.PeriodLength;
            }

            PlaySubmitDTO nextPlay = new()
            {
                PrevPlayId = currentPlay?.Id ?? -1,
                GameId = game.Id,
                TeamId = nextTeamId ?? 0,
                FieldPositionStart = fieldPositionStart,
                Down = down,
                ToGain = toGain,
                ClockStart = clockStart,
                GamePeriod = gamePeriod
            };

            GameStreamDTO gameStream = new(game, nextPlay);


            gameStream.HomeTeamScore = homeTeamScore;
            gameStream.AwayTeamScore = awayTeamScore;
            gameStream.LastPlay = lastPlay;

            // Derive player stats from the plays and populate team lists appropriately
            List<PlayerStatsDTO> playerStatsList = StatClient.ParsePlayerStats(game.Plays);
            IEnumerable<Player> allPlayers = (game.HomeTeam?.Players ?? []).Concat(game.AwayTeam?.Players ?? []);
            foreach (PlayerStatsDTO stats in playerStatsList)
            {
                stats.PlayerInfo = allPlayers.SingleOrDefault(player => player.Id == stats.PlayerId);
            }
            if (game.HomeTeam != null)
            {
                gameStream.HomeTeamPlayerStats = playerStatsList.Where(ps => game.HomeTeam.Players.Any(player => player.Id == ps.PlayerId)).ToList();
                foreach (PlayerStatsDTO stats in gameStream.HomeTeamPlayerStats)
                {
                    stats.PlayerInfo = game.HomeTeam.Players.Single(player => player.Id == stats.PlayerId);
                }
            }
            if (game.AwayTeam != null)
            {
                gameStream.AwayTeamPlayerStats = playerStatsList.Where(ps => game.AwayTeam.Players.Any(player => player.Id == ps.PlayerId)).ToList();
                
            }

            // The drive always has at least one play in it (that play may be an empty play if at start of game or currentPlay is otherwise null)
            List<Play> drive = [currentPlay ?? new()];

            bool driveFound = currentPlay?.TeamId != nextTeamId;

            // Collect all plays from current drive, including kickoff to start drive (does not count as a play)
            while (drive[0].Kickoff == null && drive[0].PrevPlayId > 0 && !driveFound)
            {
                Play? previousPlay = game.Plays.SingleOrDefault((p) => p.Id == drive[0].PrevPlayId);
                if (previousPlay == null)
                {
                    driveFound = true;
                }
                else if (previousPlay.TeamId != nextTeamId && previousPlay.Kickoff == null)
                {
                    driveFound = true;
                }
                else
                {
                    drive.Insert(0, previousPlay);
                }
            }

            // Remove kickoffs, turnovers, empty plays
            List<Play> countedPlays = drive
                .Where((p) => p.Kickoff == null
                    && p.TeamId == nextPlay.TeamId
                    && p.PrevPlayId != null)
                .ToList();

            gameStream.DrivePlays = countedPlays.Count();
            gameStream.DrivePositionStart = nextPlay.FieldPositionStart;
            gameStream.DriveYards = 0;
            gameStream.DriveTime = 0;

            if (gameStream.DrivePlays > 0)
            {
                int driveTimeStart = 0;
                int driveTimeEnd = (game.GamePeriods - (nextPlay.GamePeriod ?? 0)) * game.PeriodLength + (nextPlay.ClockStart ?? 0);

                gameStream.DrivePositionStart = countedPlays[0].FieldPositionStart;

                driveTimeStart = (game.GamePeriods - (countedPlays[0].GamePeriod ?? 0)) * game.PeriodLength + (countedPlays[0].ClockStart ?? 0);

                gameStream.DriveTime = driveTimeStart - driveTimeEnd;
                // If last play was a made field goal, or a turnover, count drive yards up to start of last play
                if ((countedPlays[countedPlays.Count - 1].FieldGoal?.Good ?? false)
                    || (nextTeamId != countedPlays[countedPlays.Count - 1].TeamId))
                {
                    gameStream.DriveYards = ((gameStream.DrivePositionStart - countedPlays[countedPlays.Count - 1].FieldPositionStart) * (currentPlay?.TeamId == game?.HomeTeamId ? -1 : 1)) ?? 0;
                }
                else if (nextPlay.Down != 0)
                {
                    gameStream.DriveYards = ((gameStream.DrivePositionStart - nextPlay.FieldPositionStart) * (currentPlay?.TeamId == game?.HomeTeamId ? -1 : 1)) ?? 0;
                }
                // else count to end
                else
                {
                    gameStream.DriveYards = ((gameStream.DrivePositionStart - countedPlays[countedPlays.Count - 1].FieldPositionEnd) * (currentPlay?.TeamId == game?.HomeTeamId ? -1 : 1)) ?? 0;
                }
            }
            gameStream.DrivePlays = countedPlays.Where((p) => !p.Penalties.Any((pp) => pp.Enforced && pp.NoPlay)).Count();

            return gameStream;
        }

        public async Task<List<PlaySegmentDTO>> GetPlaySegmentsAsync(int playId)
        {
            Play? play = await _playRepository.GetSinglePlayAsync(playId);
            if (play == null)
            {
                return [];
            }

            List<PossessionChangeDTO> chain = StatClient.GetPossessionChain(play)[0];

            int homeId = play.Game.HomeTeamId;
            int awayId = play.Game.AwayTeamId;

            Dictionary<int, string> teams = new()
            {
                {homeId, play.Game.HomeTeam.LocationName ?? play.Game.HomeTeam.Nickname },
                {awayId, play.Game.AwayTeam.LocationName ?? play.Game.AwayTeam.Nickname }
            };

            Dictionary<int, string> teamsInv = new()
            {
                {homeId, play.Game.AwayTeam.LocationName ?? play.Game.AwayTeam.Nickname},
                {awayId, play.Game.HomeTeam.LocationName ?? play.Game.HomeTeam.Nickname}
            };

            Dictionary<int, int> teamSigns = new()
            {
                {homeId, 1},
                {awayId, -1}
            };

            List<PlaySegmentDTO> segments = [];

            for (int i = 0; i < chain.Count(); i++)
            {
                if (chain[i].EntityType == typeof(Kickoff) && i > 0)
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].FromPlayerAt,
                        FieldEnd = chain[i].ToPlayerAt,
                        TeamId = play.TeamId ?? 0
                    };

                    segment.SegmentText = $"{(play.Kickoff?.Kicker != null ? $"{play.Kickoff.Kicker.Name()}" : teams[play.TeamId ?? 0])}" +
                        $" kick to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";

                    int kickYardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                    segment.SegmentText += $" for {kickYardage} yard{(Math.Abs(kickYardage) == 1 ? "" : "s")}.";

                    if (chain[i].ToPlayerId != 0)
                    {
                        segment.SegmentText += $" Fielded by {(play.Kickoff?.Returner != null ? $"{play.Kickoff.Returner.Name()}" : teamsInv[play.TeamId ?? 0])}.";
                    }
                    if (play.Kickoff.Touchback)
                    {
                        segment.SegmentText += " Touchback.";
                    }
                    segments.Add(segment);

                    // If the kickoff is a touchback, the possession chain has completed.
                    if (play.Kickoff.Touchback)
                    {
                        i = chain.Count();
                        continue;
                    }

                    // If there is another change in the chain, a return was attempted.
                    if (i + 1 < chain.Count())
                    {
                        PlaySegmentDTO returnSegment = new()
                        {
                            Index = segments.Count() + 1,
                            FieldStart = chain[i].ToPlayerAt,
                            FieldEnd = chain[i + 1].FromPlayerAt,
                            TeamId = play.TeamId == homeId ? awayId : homeId,
                        };
                        int returnYardage = (returnSegment.FieldEnd - returnSegment.FieldStart ?? 0) * teamSigns[returnSegment.TeamId];
                        returnSegment.SegmentText = $"{(play.Kickoff?.Returner != null ? $"{play.Kickoff.Returner.Name()}" : teamsInv[play.TeamId ?? 0])} " +
                            $"return for {returnYardage} yard{(Math.Abs(returnYardage) == 1 ? "" : "s")} to {StatClient.FieldPositionText(returnSegment.FieldEnd, teams[homeId], teams[awayId])}.";
                        segments.Add(returnSegment);
                        i++;
                    }
                }

                if (chain[i].EntityType == typeof(Punt) && (i > 0 || chain[i + 1].EntityType == typeof(KickBlock)))
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].FromPlayerAt,
                        FieldEnd = chain[i].ToPlayerAt,
                        TeamId = play.TeamId ?? 0,
                    };
                    string kicker = teams[segment.TeamId];
                    if (play.Punt?.Kicker != null)
                    {
                        kicker = $"{play.Punt.Kicker.Name()}";
                    }
                    segment.SegmentText += $"{kicker} punt";
                    if (i + 1 < chain.Count() && chain[i + 1].EntityType == typeof(KickBlock))
                    {
                        segment.SegmentText += " blocked";
                        if (play.KickBlock.BlockedBy != null)
                        {
                            segment.SegmentText += $" by {play.KickBlock.BlockedBy.Name()}";
                        }
                        segment.SegmentText += ".";

                        segments.Add(segment);
                        continue;
                    }
                    else
                    {
                        segment.SegmentText += $" to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";

                        int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                        segment.SegmentText += $" for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}.";

                        if (chain[i].ToPlayerId != 0)
                        {
                            segment.SegmentText += $" Fielded by {play.Punt?.Returner?.Name()}.";
                        }
                        if (play.Punt.FairCatch)
                        {
                            segment.SegmentText += " Fair catch.";
                        }
                        if (play.Punt.Touchback)
                        {
                            segment.SegmentText += " Touchback.";
                        }
                    }

                    segments.Add(segment);

                    if (play.Punt.FairCatch || play.Punt.Touchback)
                    {
                        i = chain.Count();
                        continue;
                    }

                    if (i + 1 < chain.Count())
                    {
                        PlaySegmentDTO returnSegment = new()
                        {
                            Index = segments.Count() + 1,
                            FieldStart = chain[i].ToPlayerAt,
                            FieldEnd = chain[i + 1].FromPlayerAt,
                            TeamId = play.TeamId == homeId ? awayId : homeId,
                        };
                        int returnYardage = (returnSegment.FieldEnd - returnSegment.FieldStart ?? 0) * teamSigns[returnSegment.TeamId];
                        returnSegment.SegmentText = $"{(play.Punt?.Returner != null ? $"{play.Punt.Returner.Name()}" : teamsInv[play.TeamId ?? 0])} " +
                            $"return for {returnYardage} yard{(Math.Abs(returnYardage) == 1 ? "" : "s")} to {StatClient.FieldPositionText(returnSegment.FieldEnd, teams[homeId], teams[awayId])}.";
                        segments.Add(returnSegment);
                        i++;
                    }
                }

                if (chain[i].EntityType == typeof(FieldGoal) && (i > 0 || chain[i + 1].EntityType == typeof(KickBlock)))
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].FromPlayerAt,
                        FieldEnd = chain[i].ToPlayerAt,
                        TeamId = play.TeamId ?? 0,
                    };
                    string kicker = teams[segment.TeamId];
                    if (play.FieldGoal?.Kicker != null)
                    {
                        kicker = $"{play.FieldGoal.Kicker.Name()}";
                    }
                    segment.SegmentText += $"{kicker}";
                    if (segment.FieldStart != null || segment.FieldEnd != null)
                    {
                        int distance = 60 - ((segment.FieldStart ?? (segment.FieldEnd ?? 0)) * teamSigns[segment.TeamId]) + 8;
                        segment.SegmentText += $" {distance} yard";
                    }
                    segment.SegmentText += $" field goal attempt";
                    if (play.FieldGoal.Good)
                    {
                        segment.SegmentText += " good.";
                    }
                    else if (!play.FieldGoal.Good && play.KickBlock == null)
                    {
                        segment.SegmentText += " missed.";
                    }
                    else if (i + 1 < chain.Count() && chain[i + 1].EntityType == typeof(KickBlock))
                    {
                        segment.SegmentText += " blocked";
                        if (play.KickBlock.BlockedBy != null)
                        {
                            segment.SegmentText += $" by {play.KickBlock.BlockedBy.Name()}";
                        }
                        segment.SegmentText += ".";

                        segments.Add(segment);
                        continue;
                    }
                    segments.Add(segment);
                }

                if (chain[i].EntityType == typeof(KickBlock) && chain[i].ToPlayerId != 0)
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].ToPlayerAt,
                        FieldEnd = i + 1 < chain.Count() ? chain[i + 1].FromPlayerAt : null,
                        TeamId = play.KickBlock?.RecoveredBy?.TeamId ?? 0
                    };
                    segment.SegmentText = $"Recovered by {play.KickBlock?.RecoveredBy?.Name() ?? teamsInv[segment.TeamId]}";

                    if (segment.FieldStart != null)
                    {
                        segment.SegmentText += $" at {StatClient.FieldPositionText(segment.FieldStart, teams[homeId], teams[awayId])}";
                    }
                    segment.SegmentText += ".";

                    if (segment.FieldEnd != null)
                    {
                        if (segment.TeamId == play.TeamId)
                        {
                            segment.SegmentText += " Advanced ";
                        }
                        else
                        {
                            segment.SegmentText += " Returned ";
                        }

                        int returnYardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                        segment.SegmentText += $"{returnYardage} yard{(Math.Abs(returnYardage) == 1 ? "" : "s")} to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}.";
                    }

                    segments.Add(segment);
                }

                if (((chain[i].EntityType == typeof(Pass) && (i > 0 || (!play.Pass.Completion && play.Interception == null)))
                        || chain[i].EntityType == typeof(Interception))
                    && i < chain.Count() - 1)
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = i == 0 ? chain[i].ToPlayerAt : chain[i].FromPlayerAt,
                        FieldEnd = chain[i].EntityType == typeof(Interception) ? chain[i].ToPlayerAt : chain[i + 1].FromPlayerAt,
                        TeamId = play.TeamId ?? 0,
                    };
                    string passer = teams[segment.TeamId];
                    if (play.Pass.Passer != null)
                    {
                        passer = $"{play.Pass.Passer.Name()}";
                    }
                    string receiver = "";
                    if (play.Pass.Receiver != null)
                    {
                        receiver = $"{play.Pass.Receiver.Name()}";
                    }
                    if (play.FieldGoal != null && play.FieldGoal.Fake)
                    {
                        segment.SegmentText += $"{teams[segment.TeamId]} fake field goal. ";
                    }
                    else if (play.Punt != null && play.Punt.Fake)
                    {
                        segment.SegmentText += $"{teams[segment.TeamId]} fake punt. ";
                    }
                    // An incomplete pass with tacklers and no interception is a sack
                    if (!play.Pass.Completion && play.Tacklers.Count() > 0 && play.Interception == null)
                    {
                        int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                        segment.SegmentText += $"{passer} sacked at {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])} for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}.";
                    }
                    else
                    {
                        segment.SegmentText += $"{passer} pass";
                        if (receiver != "")
                        {
                            segment.SegmentText += $" to {receiver}";
                        }
                        if (play.Interception != null)
                        {
                            segment.SegmentText += " intercepted";

                            string defender = teamsInv[segment.TeamId];
                            if (play.Interception.InterceptedBy != null)
                            {
                                defender = play.Interception.InterceptedBy.Name();
                            }
                            segment.SegmentText += $" by {defender}";
                            if (play.Interception.InterceptedAt != null)
                            {
                                segment.SegmentText += $" at {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";
                            }
                        }
                        else
                        {
                            segment.SegmentText += play.Pass.Completion ? " complete" : " incomplete";
                            if (play.Pass.Completion)
                            {
                                int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                                segment.SegmentText += $" to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])} for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}";
                            }
                        }
                        segment.SegmentText += ".";

                        if (play.PassDefenders.Count() > 0)
                        {
                            segment.SegmentText += " Broken up by ";
                            for (int j = 0; j < play.PassDefenders.Count(); j++)
                            {
                                Player? defender = play.PassDefenders[j].Defender;
                                if (defender != null)
                                {
                                    segment.SegmentText += $"{defender.Name()}";
                                }
                                else
                                {
                                    segment.SegmentText += $"PLAYER";
                                }
                                segment.SegmentText += j == play.PassDefenders.Count() - 1 ? "." : ", ";
                            }
                        }
                    }

                    segments.Add(segment);

                    if (play.Interception != null && i + 1 < chain.Count())
                    {
                        PlaySegmentDTO returnSegment = new()
                        {
                            Index = segments.Count() + 1,
                            FieldStart = chain[i].ToPlayerAt,
                            FieldEnd = chain[i + 1].FromPlayerAt,
                            TeamId = play.Interception?.InterceptedBy?.TeamId ?? 0,
                        };

                        int returnYardage = (returnSegment.FieldEnd - returnSegment.FieldStart ?? 0) * teamSigns[returnSegment.TeamId];

                        returnSegment.SegmentText = $"{(play.Interception?.InterceptedBy != null ? $"{play.Interception.InterceptedBy.Name()}" : teamsInv[play.TeamId ?? 0])} " +
                            $"return for {returnYardage} yard{(Math.Abs(returnYardage) == 1 ? "" : "s")}";
                        if (returnYardage != 0)
                        {
                            returnSegment.SegmentText += $" to {StatClient.FieldPositionText(returnSegment.FieldEnd, teams[homeId], teams[awayId])}";
                        }
                        returnSegment.SegmentText += ".";

                        segments.Add(returnSegment);
                    }
                }

                if (chain[i].EntityType == typeof(Rush) && i < chain.Count() - 1)
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].ToPlayerAt,
                        FieldEnd = chain[i + 1].FromPlayerAt,
                        TeamId = play.TeamId ?? 0,
                    };
                    if (play.FieldGoal != null && play.FieldGoal.Fake)
                    {
                        segment.SegmentText += $"{teams[segment.TeamId]} fake field goal. ";
                    }
                    else if (play.Punt != null && play.Punt.Fake)
                    {
                        segment.SegmentText += $"{teams[segment.TeamId]} fake punt. ";
                    }
                    string rusher = teams[segment.TeamId];
                    if (play.Rush?.Rusher != null)
                    {
                        rusher = $"{play.Rush.Rusher.Name()}";
                    }
                    segment.SegmentText = $"{rusher} run";
                    if (segment.FieldEnd != null)
                    {
                        segment.SegmentText += $" to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";
                        if (segment.FieldStart != null)
                        {
                            int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                            segment.SegmentText += $" for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}";
                        }
                    }
                    segment.SegmentText += ".";
                    segments.Add(segment);
                }

                if (chain[i].EntityType == typeof(Lateral))
                {
                    Lateral? lateral = play.Laterals.SingleOrDefault((l) => l.Id == chain[i].EntityId);
                    if (lateral == null)
                    {
                        continue;
                    }

                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].ToPlayerAt,
                        FieldEnd = chain[i + 1].FromPlayerAt,
                        TeamId = lateral.NewCarrier.TeamId,
                    };
                    segment.SegmentText = $"{lateral.PrevCarrier.Name()} lateral to {lateral.NewCarrier.Name()} at "
                        + $"{StatClient.FieldPositionText(segment.FieldStart, teams[homeId], teams[awayId])}.";
                    if (segment.FieldEnd != null)
                    {
                        segment.SegmentText += $" Carried to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";
                        if (segment.FieldStart != null)
                        {
                            int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                            segment.SegmentText += $" for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}";
                        }
                    }
                    segment.SegmentText += ".";
                    segments.Add(segment);
                }

                if (chain[i].EntityType == typeof(Fumble))
                {
                    Fumble? fumble = play.Fumbles.SingleOrDefault((f) => f.Id == chain[i].EntityId);
                    if (fumble == null)
                    {
                        continue;
                    }

                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].ToPlayerAt,
                        FieldEnd = chain[i + 1].FromPlayerAt,
                        TeamId = fumble.FumbleRecoveredBy?.TeamId ?? segments[^1].TeamId,
                    };

                    string addition = $" {fumble.FumbleCommittedBy.Name()} fumble";
                    if (fumble.FumbleForcedBy != null)
                    {
                        addition += $", forced by {fumble.FumbleForcedBy.Name()}";
                    }
                    addition += ".";
                    segments[^1].SegmentText += addition;

                    if (fumble.FumbleRecoveredBy == null)
                    {
                        segment.FieldStart = segment.FieldEnd;
                        segment.SegmentText = $"Ball out of bounds at {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}.";
                    }
                    else
                    {
                        segment.SegmentText = $"Fumble recovered by {fumble.FumbleRecoveredBy.Name()} at "
                            + $"{StatClient.FieldPositionText(segment.FieldStart, teams[homeId], teams[awayId])}.";

                        if (segment.TeamId == segments[^1].TeamId)
                        {
                            segment.SegmentText += " Advanced";
                        }
                        else
                        {
                            segment.SegmentText += " Returned";
                        }
                        if (segment.FieldEnd != null)
                        {
                            segment.SegmentText += $" to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";
                            if (segment.FieldStart != null)
                            {
                                int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                                segment.SegmentText += $" for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}";
                            }
                        }
                        segment.SegmentText += ".";

                    }
                    segments.Add(segment);
                }
            }


            //Append tackles
            if (play.Tacklers.Count() > 0 && segments.Count != 0)
            {
                PlaySegmentDTO segment = segments.Single(s => s.Index == segments.Count());
                segment.SegmentText += " Tackle made by ";
                for (int i = 0; i < play.Tacklers.Count(); i++)
                {
                    Player? tackler = play.Tacklers[i].Tackler;
                    if (tackler != null)
                    {
                        segment.SegmentText += $"{tackler.Name()}";
                    }
                    else
                    {
                        segment.SegmentText += $"PLAYER";
                    }
                    segment.SegmentText += i == play.Tacklers.Count() - 1 ? "." : ", ";
                }
            }

            //Account for touchdowns, extra points, two-point conversions, and defensive conversions
            if (play.Touchdown != null)
            {
                PlaySegmentDTO segment = segments[^1];
                segment.SegmentText += $" Touchdown {teams[segment.TeamId]}.";
                if (play.ExtraPoint != null)
                {
                    string kicker = play.ExtraPoint.Kicker?.Name() ?? "";
                    segment.SegmentText += $" {(kicker != "" ? $"{kicker} e" : "E")}xtra point attempt";
                    if (play.ExtraPoint.Fake)
                    {
                        segment.SegmentText += " faked.";
                    }
                    else if (play.ExtraPoint.Good)
                    {
                        segment.SegmentText += " good.";
                    }
                    else
                    {
                        segment.SegmentText += " missed.";
                    }
                }
                if (play.Conversion != null)
                {
                    segment.SegmentText += $" Two-point conversion attempt {(play.Conversion.Good ? "" : "no ")}good";
                    if (play.Conversion.Passer != null)
                    {
                        segment.SegmentText += $" ({play.Conversion.Passer.Name()} pass";
                        if (play.Conversion.Receiver != null)
                        {
                            segment.SegmentText += $" to {play.Conversion.Receiver.Name()}";
                        }
                        segment.SegmentText += $")";
                    }
                    else if (play.Conversion.Rusher != null)
                    {
                        segment.SegmentText += $" ({play.Conversion.Rusher.Name()} rush)";
                    }
                    segment.SegmentText += ".";
                }
                if ((play.ExtraPoint?.DefensiveConversion ?? false) || (play.Conversion?.DefensiveConversion ?? false))
                {
                    Player? returner = play.ExtraPoint?.Returner != null ? play.ExtraPoint.Returner : play.Conversion?.Returner;
                    segment.SegmentText += $" Returned by {(returner != null ? returner.Name() : teamsInv[segment.TeamId])} for defensive conversion.";
                }
            }

            // Account for safeties
            if (play.Safety != null)
            {
                PlaySegmentDTO segment = segments[^1];
                segment.SegmentText += $" Safety {teamsInv[segment.TeamId]}";
            }

            // Account for penalties
            foreach (PlayPenalty penalty in play.Penalties.OrderBy((p) => p.Enforced))
            {
                PlaySegmentDTO segment = new()
                {
                    Index = segments.Count() + 1,
                    FieldStart = penalty.Enforced ? penalty.EnforcedFrom : null,
                    FieldEnd = penalty.Enforced ? penalty.EnforcedFrom - (penalty.Yardage * teamSigns[penalty.TeamId]) : null,
                    TeamId = penalty.TeamId,
                    LineType = "penalty",
                    EndpointType = "penalty",
                };
                segment.SegmentText = $"{penalty.Penalty.Name}, {teams[penalty.TeamId]}";
                if (penalty.Player != null)
                {
                    segment.SegmentText += $" ({penalty.Player.NameAndNumber()})";
                }
                segment.SegmentText += ".";
                if (!penalty.Enforced)
                {
                    segment.SegmentText += " Declined.";
                }
                else
                {
                    segment.SegmentText += $" Enforced {penalty.Yardage} yard{(penalty.Yardage == 1 ? "" : "s")} from"
                        + $" {StatClient.FieldPositionText(segment.FieldStart, teams[homeId], teams[awayId])}.";
                    if (penalty.LossOfDown)
                    {
                        segment.SegmentText += $" Loss of Down.";
                    }
                    if (penalty.AutoFirstDown)
                    {
                        segment.SegmentText += $" Automatic First Down.";
                    }
                    if (penalty.NoPlay)
                    {
                        segment.SegmentText += $" No Play.";
                    }
                }

                segments.Add(segment);
            }

            return segments;
        }

        public async Task BroadcastGameStream(int gameId)
        {
            GameStreamDTO? gameStream = await GetGameStreamAsync(gameId);
            if (gameStream != null)
            {
                await _watchContext.Clients.Groups($"watch-{gameId}").UpdateGameStream(gameStream);
            }
        }

        public Task BroadcastGameStreamByPlayId(int playId)
        {
            throw new NotImplementedException();
        }
    }
}
