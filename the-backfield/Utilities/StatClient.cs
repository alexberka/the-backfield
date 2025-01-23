using TheBackfield.DTOs;
using TheBackfield.DTOs.GameStream;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Utilities
{
    public class StatClient
    {
        public static (int homeTeamScore, int awayTeamScore) ParseScore(Game gameWithPlays)
        {
            // Retrieve all plays not nullified by penalties with scoring entities
            List<Play> scoringPlays = gameWithPlays.Plays
                .Where(p => (p.Touchdown != null || p.Safety != null || (p.FieldGoal != null && p.FieldGoal.Good))
                            && !p.Penalties.Any(pe => pe.Enforced == true && pe.NoPlay == true))
                .ToList();

            List<int> scores = [];

            // Analyze each play, assigning positive values to home team scores, negative values to away team scores
            foreach (Play play in scoringPlays)
            {
                if (play.Touchdown != null)
                {
                    // Add 6 for touchdowns
                    scores.Add(play.FieldPositionEnd == 50 ? 6 : -6);
                    // Add 1 for extra points
                    if (play.ExtraPoint != null && play.ExtraPoint.Good)
                    {
                        scores.Add(play.FieldPositionEnd == 50 ? 1 : -1);
                    }
                    // Add 2 for successful conversions
                    if (play.Conversion != null && play.Conversion.Good)
                    {
                        scores.Add(play.FieldPositionEnd == 50 ? 2 : -2);
                    }
                    // Add 2 for defense on defensive conversions
                    if ((play.ExtraPoint != null && play.ExtraPoint.DefensiveConversion)
                        || (play.Conversion != null && play.Conversion.DefensiveConversion))
                    {
                        scores.Add(play.FieldPositionEnd == 50 ? -2 : 2);
                    }
                }
                else if (play.FieldGoal != null && play.FieldGoal.Good)
                {
                    // Add 3 for field goals
                    scores.Add(play.FieldPositionEnd == 50 ? 3 : -3);
                }
                else if (play.Safety != null)
                {
                    // Add 2 for safeties
                    scores.Add(play.FieldPositionEnd == 50 ? 2 : -2);
                }
            }
            // Add up positive scores for home team score, negative scores (* -1) for away team score, return tuple
            return (scores.Sum(v => v > 0 ? v : 0), scores.Sum(v => v < 0 ? -1 * v : 0));
        }

        public static (int nextDown, int? nextToGain, int? nextFieldPosition, int? nextTeamId) ParseNextFieldPosition(Play play, int homeTeamId, int awayTeamId)
        {
            int? fieldPositionEnd = play.FieldPositionEnd;

            // Factor in penalties to calculate line of scrimmage
            List<PlayPenalty> enforcedPenalties = play.Penalties.Where(pe => pe.Enforced == true).ToList();
            if (enforcedPenalties.Count() > 0)
            {
                fieldPositionEnd = enforcedPenalties[0].EnforcedFrom;
                foreach (PlayPenalty penalty in enforcedPenalties)
                {
                    int penaltyTeamSign = penalty.TeamId == homeTeamId ? -1 : 1;
                    fieldPositionEnd += penaltyTeamSign * penalty.Yardage;
                    if (Math.Abs(fieldPositionEnd ?? 0) > 50)
                    {
                        fieldPositionEnd = penaltyTeamSign * 49;
                    }
                }
            }
            // If no penalties and the kickoff has resulted in a touchback, move ball to receiving team's 30-yard-line
            // The assumption is that correct field position will result from penalty enforcement otherwise
            else if (play.Kickoff != null && play.Kickoff.Touchback)
            {
                fieldPositionEnd = 20 * (play.TeamId == homeTeamId ? 1 : -1);
            }

            int? nextFieldPositionStart = fieldPositionEnd;
            int teamId = play.TeamId ?? 0;
            int down = play.Down;
            int? toGain = play.ToGain;

            Dictionary<int, int> teamSigns = new()
            {
                { homeTeamId, 1 },
                { awayTeamId, -1 }
            };

            // If a penalty results in no play and is not a loss of down penalty
            if (enforcedPenalties.Any(ep => ep.NoPlay == true) && !enforcedPenalties.Any(ep => ep.LossOfDown == true))
            {
                // If penalty is an automatic first down penalty or results in first down, then adjust down and line to gain
                if (play.Kickoff == null && (enforcedPenalties.Any(ep => ep.AutoFirstDown == true) || (fieldPositionEnd - toGain) * teamSigns[teamId] >= 0))
                {
                    down = 1;
                    toGain = Math.Abs((nextFieldPositionStart + (teamSigns[teamId] * 10)) ?? 0) > 50 ? teamSigns[teamId] * 50 : nextFieldPositionStart + (teamSigns[teamId] * 10);
                }
            }
            else if (play.Safety != null || play.Touchdown != null || play.FieldGoal?.Good == true)
            {
                // If a team scores, the next play will be a free kick
                down = 0;
                toGain = null;
                if (play.Safety != null)
                {
                    // If a safety is scored, the kicking team is the team that was scored on, kicking from own 20
                    if (fieldPositionEnd == 50)
                    {
                        teamId = awayTeamId;
                    }
                    else if (fieldPositionEnd == -50)
                    {
                        teamId = homeTeamId;
                    }
                    nextFieldPositionStart = -30 * teamSigns[teamId];
                }
                else
                {
                    // Else kicking team is scoring team, kicking from own 35
                    if (fieldPositionEnd == 50)
                    {
                        teamId = homeTeamId;
                    }
                    else if (fieldPositionEnd == -50)
                    {
                        teamId = awayTeamId;
                    }
                    nextFieldPositionStart = -15 * teamSigns[teamId];
                }
            }
            else
            {
                // If the play was a kickoff or punt or missed field goal, possession changes
                // This change may be undone by a fumble or blocked kick, which is why this code block exists
                // inside this else statement and not as a separate else if
                if (play.Kickoff != null
                    || (play.Punt != null && !play.Punt.Fake)
                    || (play.FieldGoal != null && !play.FieldGoal.Fake && !play.FieldGoal.Good))
                {
                    teamId = teamId == homeTeamId ? awayTeamId : homeTeamId;
                }
                // If play included a fumble, blocked kick, or interception, examine possession chain for new possession team
                if (play.Fumbles.Count() > 0 || play.KickBlock != null || play.Interception != null)
                {
                    List<Player?> gainsPossession = [];
                    List<Player?> cedesPossession = [];

                    gainsPossession.Add(play.Pass?.Passer);
                    gainsPossession.Add(play.Pass?.Completion ?? false ? play.Pass?.Receiver ?? null : null);
                    cedesPossession.Add(play.Pass?.Completion ?? false ? play.Pass?.Passer ?? null : null);
                    gainsPossession.Add(play.Rush?.Rusher);
                    gainsPossession.Add(play.Punt?.Returner);
                    gainsPossession.Add(play.Kickoff?.Returner);
                    gainsPossession.Add(play.Interception?.InterceptedBy);
                    cedesPossession.Add(play.Interception != null ? play.Pass?.Passer ?? null : null);
                    gainsPossession.Add(play.KickBlock?.RecoveredBy);
                    foreach (Fumble fumble in play.Fumbles)
                    {
                        cedesPossession.Add(fumble.FumbleCommittedBy);
                        gainsPossession.Add(fumble?.FumbleRecoveredBy);
                    }
                    foreach (Lateral lateral in play.Laterals)
                    {
                        cedesPossession.Add(lateral.PrevCarrier);
                        gainsPossession.Add(lateral.NewCarrier);
                    }
                    gainsPossession.RemoveAll(p => p == null);
                    cedesPossession.RemoveAll(p => p == null);

                    foreach (Player? carrier in cedesPossession)
                    {
                        if (carrier != null && gainsPossession.Count((p) => p?.Id == carrier.Id) != 0)
                        {
                            var player = gainsPossession
                                .Select((p, index) => new { p?.Id, Index = index })
                                .SingleOrDefault((item) => item.Id == carrier.Id);
                            gainsPossession.RemoveAt(player?.Index ?? -1);
                        }
                    }
                    if (gainsPossession.Count() == 1)
                    {
                        teamId = gainsPossession[0]?.TeamId ?? 0;
                    }

                    // If no players remain with possession, check for unrecovered fumble
                    if (gainsPossession.Count == 0)
                    {
                        Fumble? notRecovered = play.Fumbles.SingleOrDefault(f => f.FumbleRecoveredById == null);
                        if (notRecovered != null)
                        {
                            if (Math.Abs(play.FieldPositionEnd ?? 0) == 50)
                            {
                                teamId = notRecovered.FumbleCommittedBy.TeamId == homeTeamId ? awayTeamId : homeTeamId;
                            }
                            else
                            {
                                teamId = notRecovered.FumbleCommittedBy.TeamId;
                            }
                        }
                    }
                }
                // If the play has resulted in a possession change, reset down and line to gain
                if (teamId != play.TeamId && teamId != 0)
                {
                    down = 1;
                    toGain = Math.Abs(nextFieldPositionStart + (teamSigns[teamId] * 10) ?? 0) > 50 ? teamSigns[teamId] * 50 : nextFieldPositionStart + (teamSigns[teamId] * 10);
                }
                // Otherwise evaluate for first down
                else
                {
                    if ((nextFieldPositionStart - toGain) * teamSigns[teamId] >= 0)
                    {
                        down = 1;
                        toGain = Math.Abs(nextFieldPositionStart + (teamSigns[teamId] * 10) ?? 0) > 50 ? teamSigns[teamId] * 50 : nextFieldPositionStart + (teamSigns[teamId] * 10);
                    }
                    else if (play.Down < 4)
                    {
                        down = play.Down + 1;
                    }
                    // If the line to gain was not met on a 4th down, ball is turned over
                    else
                    {
                        teamId = teamId == homeTeamId ? awayTeamId : homeTeamId;
                        down = 1;
                        toGain = Math.Abs(nextFieldPositionStart + (teamSigns[teamId] * 10) ?? 0) > 50 ? teamSigns[teamId] * 50 : nextFieldPositionStart + (teamSigns[teamId] * 10);
                    }
                }
            }
            
            return (down, toGain, nextFieldPositionStart, teamId);
        }

        public static string FieldPositionText(int? fieldPosition, string homeTeam, string awayTeam)
        {
            if (fieldPosition == null)
            {
                return "";
            }
            string asText = "";
            if (fieldPosition > 0)
            {
                asText += $"{awayTeam} ";
            }
            else if (fieldPosition < 0)
            {
                asText += $"{homeTeam} ";
            }
            int fieldNumber = 50 - Math.Abs(fieldPosition ?? 0);
            if (fieldNumber <= 0)
            {
                asText += "endzone";
            }
            else
            {
                asText += $"{fieldNumber}";
            }

            return asText;
        }

        public static List<PlayerStatsDTO> ParsePlayerStats(List<Play> plays)
        {
            List<PlayerStatsDTO> statsList = [];

            foreach (Play play in plays.Where(p => !p.Penalties.Any(penalty => penalty.Enforced && penalty.NoPlay)))
            {
                if (play.Pass != null)
                {
                    if (play.Pass.PasserId != null)
                    {
                        PlayerStatsDTO? passerStats = statsList.SingleOrDefault(ps => ps.PlayerId == play.Pass.PasserId);
                        if (passerStats == null)
                        {
                            passerStats = new() { PlayerId = (int)play.Pass.PasserId };
                            statsList.Add(passerStats);
                        }
                        if (!play.Pass.Sack)
                        {
                            passerStats.PassAttempts++;
                        }
                        if (play.Pass.Completion)
                        {
                            passerStats.PassCompletions++;
                            passerStats.PassYards += play.Pass.PassYardage;
                            if (play.Touchdown?.TeamId == play.TeamId)
                            {
                                passerStats.PassTouchdowns++;
                            }
                        }
                        if (play.Interception != null)
                        {
                            passerStats.InterceptionsThrown++;
                            if (play.Interception.InterceptedById != null)
                            {
                                PlayerStatsDTO? interceptStats = statsList.SingleOrDefault(ps => ps.PlayerId == play.Interception.InterceptedById);
                                if (interceptStats == null)
                                {
                                    interceptStats = new() { PlayerId = (int)play.Interception.InterceptedById };
                                    statsList.Add(interceptStats);
                                }
                                interceptStats.InterceptionsReceived++;
                                interceptStats.InterceptionReturnYards += play.Interception.ReturnYardage;
                                if (play.Touchdown?.PlayerId == interceptStats.PlayerId)
                                {
                                    interceptStats.InterceptionReturnTouchdowns++;
                                }
                            }
                        }
                    }
                    if (play.Pass.ReceiverId != null)
                    {
                        PlayerStatsDTO? receiverStats = statsList.SingleOrDefault(ps => ps.PlayerId == play.Pass.ReceiverId);
                        if (receiverStats == null)
                        {
                            receiverStats = new() { PlayerId = (int)play.Pass.ReceiverId };
                            statsList.Add(receiverStats);
                        }
                        receiverStats.ReceivingTargets++;
                        if (play.Pass.Completion)
                        {
                            receiverStats.Receptions++;
                            receiverStats.ReceivingYards += play.Pass.ReceptionYardage;
                        }
                        if (play.Touchdown?.PlayerId == receiverStats.PlayerId)
                        {
                            receiverStats.ReceivingTouchdowns++;
                        }
                    }
                }
                if (play.Rush != null && play.Rush.RusherId != null)
                {
                    PlayerStatsDTO? rusherStats = statsList.SingleOrDefault(ps => ps.PlayerId == play.Rush.RusherId);
                    if (rusherStats == null)
                    {
                        rusherStats = new() { PlayerId = (int)play.Rush.RusherId };
                        statsList.Add(rusherStats);
                    }
                    rusherStats.RushAttempts++;
                    rusherStats.RushYards += play.Rush.Yardage;
                    if (play.Touchdown != null && play.Touchdown.PlayerId == rusherStats.PlayerId)
                    {
                        rusherStats.RushTouchdowns++;
                    }
                }
                foreach (Tackle tackler in play.Tacklers)
                {
                    if (tackler.TacklerId != null)
                    {
                        PlayerStatsDTO? tacklerStats = statsList.SingleOrDefault(ps => ps.PlayerId == tackler.TacklerId);
                        if (tacklerStats == null)
                        {
                            tacklerStats = new() { PlayerId = (int)tackler.TacklerId };
                            statsList.Add(tacklerStats);
                        }
                        tacklerStats.Tackles++;
                        if (play.Tacklers.Count == 1)
                        {
                            tacklerStats.SoloTackles++;
                        }
                        // Check for tackle for loss (can only occur on scrimmage play)
                        if (play.TeamId != tackler.TeamId && play.ToGain != null)
                        {
                            // If the line of scrimmage is less than the line to gain, then the home team has the ball
                            bool homeTeamPossession = play.FieldPositionStart < play.ToGain;
                            if ((homeTeamPossession && play.FieldPositionEnd < play.FieldPositionStart)
                                || (!homeTeamPossession && play.FieldPositionEnd > play.FieldPositionStart))
                            {
                                tacklerStats.TacklesForLoss++;
                            }
                        }
                        // Player records a sack if the sack ended the play (no fumble, tackles must have occurred at the end of the play)
                        // or the tackler caused the passer to fumble (strip sack)
                        if (play.Pass != null && 
                                ((play.Pass.Sack && play.Fumbles.Count == 0)
                                || play.Fumbles.Any(f => f.FumbleCommittedById == play.Pass.PasserId && f.FumbleForcedById == tacklerStats.PlayerId)))
                        {
                            tacklerStats.Sacks += play.Tacklers.Count == 1 ? 1 : 0.5;
                        }
                    }
                }
                foreach (Fumble fumble in play.Fumbles)
                {
                    if (fumble.FumbleCommittedById != null)
                    {
                        PlayerStatsDTO? fumblerStats = statsList.SingleOrDefault(ps => ps.PlayerId == fumble.FumbleCommittedById);
                        if (fumblerStats == null)
                        {
                            fumblerStats = new() { PlayerId = (int)fumble.FumbleCommittedById };
                            statsList.Add(fumblerStats);
                        }
                        fumblerStats.FumblesCommitted++;
                    }
                    if (fumble.FumbleForcedById != null)
                    {
                        PlayerStatsDTO? forcedStats = statsList.SingleOrDefault(ps => ps.PlayerId == fumble.FumbleForcedById);
                        if (forcedStats == null)
                        {
                            forcedStats = new() { PlayerId = (int)fumble.FumbleForcedById };
                            statsList.Add(forcedStats);
                        }
                        forcedStats.FumblesForced++;
                    }
                    if (fumble.FumbleRecoveredById != null)
                    {
                        PlayerStatsDTO? recoveredStats = statsList.SingleOrDefault(ps => ps.PlayerId == fumble.FumbleRecoveredById);
                        if (recoveredStats == null)
                        {
                            recoveredStats = new() { PlayerId = (int)fumble.FumbleRecoveredById };
                            statsList.Add(recoveredStats);
                        }
                        recoveredStats.FumblesRecovered++;
                        if (fumble.YardageType == "return")
                        {
                            recoveredStats.FumbleReturnYards += fumble.ReturnYardage;
                            if (play.Touchdown?.PlayerId == recoveredStats.PlayerId)
                            {
                                recoveredStats.FumbleReturnTouchdowns++;
                            }
                        }
                    }
                }
                if (play.FieldGoal != null && !play.FieldGoal.Fake && play.FieldGoal.KickerId != null)
                {
                    PlayerStatsDTO? kickerStats = statsList.SingleOrDefault(ps => ps.PlayerId == play.FieldGoal.KickerId);
                    if (kickerStats == null)
                    {
                        kickerStats = new() { PlayerId = (int)play.FieldGoal.KickerId };
                        statsList.Add(kickerStats);
                    }
                    kickerStats.FieldGoalAttempts++;
                    kickerStats.FieldGoalsMade += play.FieldGoal.Good ? 1 : 0;
                }
                if (play.ExtraPoint != null && !play.ExtraPoint.Fake && play.ExtraPoint.KickerId != null)
                {
                    PlayerStatsDTO? kickerStats = statsList.SingleOrDefault(ps => ps.PlayerId == play.ExtraPoint.KickerId);
                    if (kickerStats == null)
                    {
                        kickerStats = new() { PlayerId = (int)play.ExtraPoint.KickerId };
                        statsList.Add(kickerStats);
                    }
                    kickerStats.ExtraPointAttempts++;
                    kickerStats.ExtraPointsMade += play.ExtraPoint.Good ? 1 : 0;
                }
                if (play.Punt != null && !play.Punt.Fake)
                {
                    if (play.Punt.KickerId != null)
                    {
                        PlayerStatsDTO? kickerStats = statsList.SingleOrDefault(ps => ps.PlayerId == play.Punt.KickerId);
                        if (kickerStats == null)
                        {
                            kickerStats = new() { PlayerId = (int)play.Punt.KickerId };
                            statsList.Add(kickerStats);
                        }
                        kickerStats.Punts++;
                        kickerStats.PuntYards += play.Punt.Distance;
                    }
                    if (play.Punt.ReturnerId != null)
                    {
                        PlayerStatsDTO? returnerStats = statsList.SingleOrDefault(ps => ps.PlayerId == play.Punt.ReturnerId);
                        if (returnerStats == null)
                        {
                            returnerStats = new() { PlayerId = (int)play.Punt.ReturnerId };
                            statsList.Add(returnerStats);
                        }
                        returnerStats.PuntReturns++;
                        returnerStats.PuntReturnYards += play.Punt.ReturnYardage;
                        if (play.Touchdown?.PlayerId == returnerStats.PlayerId)
                        {
                            returnerStats.PuntReturnTouchdowns++;
                        }
                    }
                }
                if (play.Kickoff != null)
                {
                    if (play.Kickoff.KickerId != null)
                    {
                        PlayerStatsDTO? kickerStats = statsList.SingleOrDefault(ps => ps.PlayerId == play.Kickoff.KickerId);
                        if (kickerStats == null)
                        {
                            kickerStats = new() { PlayerId = (int)play.Kickoff.KickerId };
                            statsList.Add(kickerStats);
                        }
                        kickerStats.Kickoffs++;
                        kickerStats.KickoffsForTouchbacks += play.Kickoff.Touchback ? 1 : 0;
                    }
                    if (play.Kickoff.ReturnerId != null)
                    {
                        PlayerStatsDTO? returnerStats = statsList.SingleOrDefault(ps => ps.PlayerId == play.Kickoff.ReturnerId);
                        if (returnerStats == null)
                        {
                            returnerStats = new() { PlayerId = (int)play.Kickoff.ReturnerId };
                            statsList.Add(returnerStats);
                        }
                        returnerStats.KickoffReturns++;
                        returnerStats.KickoffReturnYards += play.Kickoff.ReturnYardage;
                        if (play.Touchdown?.PlayerId == returnerStats.PlayerId)
                        {
                            returnerStats.KickoffReturnTouchdowns++;
                        }
                    }
                }
                foreach (Lateral lateral in play.Laterals)
                {
                    if (lateral.NewCarrierId != null)
                    {
                        PlayerStatsDTO? newCarrierStats = statsList.SingleOrDefault(ps => ps.PlayerId == lateral.NewCarrierId);
                        if (newCarrierStats == null)
                        {
                            newCarrierStats = new() { PlayerId = (int)lateral.NewCarrierId };
                            statsList.Add(newCarrierStats);
                        }
                        if (lateral.YardageType == "pass")
                        {
                            newCarrierStats.ReceivingYards += lateral.Yardage;
                            if (play.Touchdown?.PlayerId == newCarrierStats.PlayerId)
                            {
                                newCarrierStats.ReceivingTouchdowns++;
                            }
                        }
                        else if (lateral.YardageType == "rush")
                        {
                            newCarrierStats.RushYards += lateral.Yardage;
                            if (play.Touchdown?.PlayerId == newCarrierStats.PlayerId)
                            {
                                newCarrierStats.RushTouchdowns++;
                            }
                        }
                    }
                }
            }

            return statsList;
        }

        public static List<List<PossessionChangeDTO>> GetPossessionChain(Play play)
        {
            // No possession chain when play does not contain a pass, rush, kickoff, field goal, or punt
            if (play.Pass == null && play.Rush == null && play.Kickoff == null && play.FieldGoal == null && play.Punt == null)
            {
                return [[]];
            }
            List<PossessionChangeDTO> possessionChanges = [];

            // If a play was a kick and not a fake, start with kick info
            if (play.Kickoff != null
                || (play.Punt != null && !play.Punt.Fake)
                || (play.FieldGoal != null && !play.FieldGoal.Fake))
            {
                if (play.Kickoff != null)
                {
                    possessionChanges.Add(new()
                    {
                        ToPlayerId = play.Kickoff.KickerId ?? 0,
                        ToPlayerAt = play.FieldPositionStart,
                        EntityType = typeof(Kickoff),
                        EntityId = play.Kickoff.Id
                    });
                    possessionChanges.Add(new()
                    {
                        FromPlayerId = play.Kickoff.KickerId ?? 0,
                        ToPlayerId = play.Kickoff.ReturnerId ?? 0,
                        FromPlayerAt = play.FieldPositionStart,
                        ToPlayerAt = play.Kickoff.FieldedAt ?? play.FieldPositionEnd,
                        EntityType = typeof(Kickoff),
                        EntityId = play.Kickoff.Id
                    });
                }
                else if (play.Punt != null)
                {
                    possessionChanges.Add(new()
                    {
                        ToPlayerId = play.Punt.KickerId ?? 0,
                        ToPlayerAt = play.FieldPositionStart,
                        EntityType = typeof(Punt),
                        EntityId = play.Punt.Id
                    });
                    if (play.KickBlock == null)
                    {
                        possessionChanges.Add(new()
                        {
                            FromPlayerId = play.Punt.KickerId ?? 0,
                            ToPlayerId = play.Punt.ReturnerId ?? 0,
                            FromPlayerAt = play.FieldPositionStart,
                            ToPlayerAt = play.Punt.FieldedAt ?? play.FieldPositionEnd,
                            EntityType = typeof(Punt),
                            EntityId = play.Punt.Id
                        });
                    }
                }
                else if (play.FieldGoal != null)
                {
                    possessionChanges.Add(new()
                    {
                        ToPlayerId = play.FieldGoal.KickerId ?? 0,
                        ToPlayerAt = play.FieldPositionStart,
                        EntityType = typeof(FieldGoal),
                        EntityId = play.FieldGoal.Id
                    });
                    if (play.KickBlock == null)
                    {
                        possessionChanges.Add(new()
                        {
                            FromPlayerId = play.FieldGoal.KickerId ?? 0,
                            FromPlayerAt = play.FieldPositionStart,
                            ToPlayerAt = play.FieldPositionEnd,
                            EntityType = typeof(FieldGoal),
                            EntityId = play.FieldGoal.Id
                        });
                    }
                }

                if (play.KickBlock != null)
                {
                    possessionChanges.Add(new()
                    {
                        FromPlayerId = possessionChanges[0].ToPlayerId,
                        ToPlayerId = play.KickBlock.RecoveredById ?? 0,
                        FromPlayerAt = play.FieldPositionStart,
                        ToPlayerAt = play.KickBlock.RecoveredAt ?? play.FieldPositionEnd,
                        EntityType = typeof(KickBlock),
                        EntityId = play.KickBlock.Id
                    });
                }
            }
            // Otherwise assess pass data
            else if (play.Pass != null)
            {
                possessionChanges.Add(new()
                {
                    ToPlayerId = play.Pass.PasserId ?? 0,
                    ToPlayerAt = play.FieldPositionStart,
                    EntityType = typeof(Pass),
                    EntityId = play.Pass.Id,
                });
                if (play.Pass.Completion)
                {
                    possessionChanges.Add(new()
                    {
                        FromPlayerId = play.Pass.PasserId ?? 0,
                        ToPlayerId = play.Pass.ReceiverId ?? 0,
                        FromPlayerAt = play.FieldPositionStart,
                        ToPlayerAt = play.FieldPositionStart,
                        EntityType = typeof(Pass),
                        EntityId = play.Pass.Id,
                    });
                }
                else if (play.Interception != null)
                {
                    possessionChanges.Add(new()
                    {
                        FromPlayerId = play.Pass.PasserId ?? 0,
                        ToPlayerId = play.Interception.InterceptedById ?? 0,
                        FromPlayerAt = play.FieldPositionStart,
                        ToPlayerAt = play.Interception.InterceptedAt,
                        EntityType = typeof(Interception),
                        EntityId = play.Interception.Id,
                    });
                }
            }
            // Otherwise assess rush data
            else if (play.Rush != null)
            {
                possessionChanges.Add(new()
                {
                    ToPlayerId = play.Rush.RusherId ?? 0,
                    ToPlayerAt = play.FieldPositionStart,
                    EntityType = typeof(Rush),
                    EntityId = play.Rush.Id,
                });
            }
            if (possessionChanges[^1].ToPlayerId != 0)
            {
                possessionChanges.Add(new()
                {
                    FromPlayerId = possessionChanges[^1].ToPlayerId,
                    FromPlayerAt = play.FieldPositionEnd,
                    EntityType = possessionChanges[^1].EntityType,
                    EntityId = possessionChanges[^1].EntityId,
                });
            }

            List<List<PossessionChangeDTO>> possessionChains = [[]];

            // If there are fumbles or laterals, build possible possession chains
            if (play.Fumbles.Count() != 0 || play.Laterals.Count() != 0)
            {
                List<PossessionChangeDTO> toPlace = [];

                foreach (Fumble fumble in play.Fumbles)
                {
                    if (fumble.FumbleCommittedById != fumble.FumbleRecoveredById)
                    {
                        toPlace.Add(new()
                        {
                            FromPlayerId = fumble.FumbleCommittedById ?? 0,
                            ToPlayerId = fumble.FumbleRecoveredById ?? 0,
                            FromPlayerAt = fumble.FumbledAt, 
                            ToPlayerAt = fumble.RecoveredAt,
                            EntityType = typeof(Fumble),
                            EntityId = fumble.Id,
                        });
                    }
                };

                foreach (Lateral lateral in play.Laterals)
                {
                    toPlace.Add(new()
                    {
                        FromPlayerId = lateral.PrevCarrierId ?? 0,
                        ToPlayerId = lateral.NewCarrierId ?? 0,
                        FromPlayerAt = lateral.PossessionAt,
                        ToPlayerAt = lateral.PossessionAt,
                        EntityType = typeof(Lateral),
                        EntityId = lateral.Id,
                    });
                };
            
                // The variable 'paths' stores a list of possible ways to combine the fumbles and laterals in toPlace with possessionChanges
                // Each path is a list of indices referencing one of the PossessionChangeDTOs in toPlace (indices are adjusted to start at 1 rather than 0)
                // The indices in the path are listed in order of addition to the possible chain, with negative indices occurring before the contents of possessionChanges
                // Ex. if toPlace contains 3 PossessionChangeDTOs, a possible path could be [3, 1, -2] which defines a path starting at the 2nd item in toPlace (toPlace[1]),
                // continuing through the contents of possessionChanges, then toPlace[2], and finally toPlace[0]
                List<List<int>> paths = [[]];

                //checkPlacementLayer is used recursively to examine possible possession chains involving the fumbles and laterals in this play
                bool checkPlacementLayer(int compareLength)
                {
                    if (toPlace.Count() == 0)
                    {
                        return true;
                    }

                    List<List<int>> activePaths = paths.Where((p) => p.Count() == compareLength - 1).ToList();

                    foreach (List<int> path in activePaths)
                    {
                        int afterPlayerCompare = possessionChanges[possessionChanges.Count() - 1].FromPlayerId;
                        int beforePlayerCompare = possessionChanges[0].ToPlayerId;
                        // If this current path has any fumbles/laterals appended to the end of possessionChanges
                        if (path.Any((index) => index > 0))
                        {
                            // Retrieve the path's positive indices
                            List<int> afterChangeIndices = path.Where((index) => index > 0).ToList();
                            // Use to locate the final ball carrier in the possible path
                            afterPlayerCompare = toPlace[afterChangeIndices[afterChangeIndices.Count() - 1] - 1].ToPlayerId;
                        }
                        // If this current path has any fumbles/laterals at the beginning of possessionChanges
                        if (path.Any((index) => index < 0))
                        {
                            // Retrieve the path's negative indices
                            List<int> beforeChangeIndices = path.Where((index) => index < 0).ToList();
                            // Use to locate the initial ball carrier in the possible path
                            beforePlayerCompare = toPlace[Math.Abs(beforeChangeIndices[beforeChangeIndices.Count() - 1]) - 1].FromPlayerId;
                        }

                        // unplaced contains only PossessionChangeDTOs that have yet to be included in this path
                        List<PossessionChangeDTO> unplaced = toPlace
                            .Where((change) => !path.Any((index) => Math.Abs(index) - 1 == toPlace.IndexOf(change)))
                            .ToList();

                        foreach (PossessionChangeDTO change in unplaced)
                        {
                            // If the change could go at the end of the chain defined by this path, add it
                            if (change.FromPlayerId == afterPlayerCompare)
                            {
                                // If the path has already added another change, a diverging path must be created with this new change
                                if (path.Count() == compareLength)
                                {
                                    List<int> divergence = path.Take(path.Count() - 1).ToList();
                                    divergence.Add(toPlace.IndexOf(change) + 1);
                                    paths.Add(divergence);
                                }
                                else
                                {
                                    path.Add(toPlace.IndexOf(change) + 1);
                                }
                            }
                            // Likewise if change could go first, but with negative indices and .Insert() method
                            if (change.ToPlayerId == beforePlayerCompare)
                            {
                                if (path.Count() == compareLength)
                                {
                                    List<int> divergence = path.Skip(1).ToList();
                                    divergence.Add(-1 * (toPlace.IndexOf(change) + 1));
                                    paths.Add(divergence);
                                }
                                else
                                {
                                    path.Add(-1 * (toPlace.IndexOf(change) + 1));
                                }
                            }
                        }
                    }

                    // If a valid path has been recorded that is equal to this cycle's compareLength then move to next one
                    // or exit recursion if all PossessionChangeDTOs have been placed
                    if (paths.Any((path) => path.Count() == compareLength))
                    {
                        if (compareLength == toPlace.Count())
                        {
                            return true;
                        }
                        return checkPlacementLayer(compareLength + 1);
                    }

                    // Otherwise terminate recursion with invalid result
                    return false;
                }

                checkPlacementLayer(1);

                possessionChains = paths
                    .Where((path) => path.Count() == toPlace.Count())
                    .Select((path) =>
                    {
                        List<PossessionChangeDTO> newChain = possessionChanges;
                        foreach (int index in path)
                        {
                            PossessionChangeDTO addChange = toPlace[Math.Abs(index) - 1];
                            if (index > 0)
                            {
                                newChain[newChain.Count() - 1].FromPlayerId = addChange.ToPlayerId;
                                newChain.Insert(newChain.Count() - 1, addChange);
                            }
                            else
                            {
                                newChain[0].ToPlayerId = addChange.FromPlayerId;
                                newChain.Insert(1, addChange);
                            }
                        }
                        return newChain;
                    })
                    .ToList();
            }
            else
            {
                possessionChains = [possessionChanges];
            }

            return possessionChains;
        }
    }
}
