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
            List<Play> scoringPlays = gameWithPlays.Plays
                .Where(p => (p.Touchdown != null || p.Safety != null || (p.FieldGoal != null && p.FieldGoal.Good))
                            && !p.Penalties.Any(pe => pe.Enforced == true && pe.NoPlay == true))
                .ToList();

            List<int> scores = [];

            foreach (Play play in scoringPlays)
            {
                if (play.Touchdown != null)
                {
                    scores.Add(play.FieldPositionEnd == 50 ? 6 : -6);
                    if (play.ExtraPoint != null && play.ExtraPoint.Good)
                    {
                        scores.Add(play.FieldPositionEnd == 50 ? 1 : -1);
                    }
                    if (play.Conversion != null && play.Conversion.Good)
                    {
                        scores.Add(play.FieldPositionEnd == 50 ? 2 : -2);
                    }
                    if ((play.ExtraPoint != null && play.ExtraPoint.DefensiveConversion)
                        || (play.ExtraPoint != null && play.ExtraPoint.DefensiveConversion))
                    {
                        scores.Add(play.FieldPositionEnd == 50 ? -2 : 2);
                    }
                }
                else if (play.FieldGoal != null && play.FieldGoal.Good)
                {
                    scores.Add(play.FieldPositionEnd == 50 ? 3 : -3);
                }
                else if (play.Safety != null)
                {
                    scores.Add(play.FieldPositionEnd == 50 ? 2 : -2);
                }
            }

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
                // If the play was a kickoff or punt, possession changes
                if (play.Kickoff != null || (play.Punt != null && !play.Punt.Fake))
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
                        if (gainsPossession.Contains(carrier))
                        {
                            gainsPossession.Remove(carrier);
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
            List<PlayerStatsDTO> playerStats = [];

            return playerStats;
        }

        public static List<List<PossessionChangeDTO>> GetPossessionChain(Play play)
        {
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
                        EntityType = typeof(Kickoff),
                        EntityId = play.Kickoff.Id
                    });
                    possessionChanges.Add(new()
                    {
                        FromPlayerId = play.Kickoff.KickerId ?? 0,
                        ToPlayerId = play.Kickoff.ReturnerId ?? 0,
                        BallReceivedAt = play.Kickoff.FieldedAt,
                        EntityType = typeof(Kickoff),
                        EntityId = play.Kickoff.Id
                    });
                }
                else if (play.Punt != null)
                {
                    possessionChanges.Add(new()
                    {
                        ToPlayerId = play.Punt.KickerId ?? 0,
                        EntityType = typeof(Punt),
                        EntityId = play.Punt.Id
                    });
                    if (play.KickBlock == null)
                    {
                        possessionChanges.Add(new()
                        {
                            FromPlayerId = play.Punt.KickerId ?? 0,
                            ToPlayerId = play.Punt.ReturnerId ?? 0,
                            BallReceivedAt = play.Punt.FieldedAt,
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
                        EntityType = typeof(FieldGoal),
                        EntityId = play.FieldGoal.Id
                    });
                    if (play.KickBlock == null)
                    {
                        possessionChanges.Add(new()
                        {
                            FromPlayerId = play.FieldGoal.KickerId ?? 0,
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
                        BallReceivedAt = play.KickBlock.RecoveredAt,
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
                    EntityType = typeof(Pass),
                    EntityId = play.Pass.Id,
                });
                if (play.Pass.Completion)
                {
                    possessionChanges.Add(new()
                    {
                        FromPlayerId = play.Pass.PasserId ?? 0,
                        ToPlayerId = play.Pass.ReceiverId ?? 0,
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
                        BallReceivedAt = play.Interception.InterceptedAt,
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
                    EntityType = typeof(Rush),
                    EntityId = play.Rush.Id,
                });
            }

            // If there are no fumbles or laterals, return current possessionChanges as only possible possession chain
            if (play.Fumbles.Count() == 0 && play.Laterals.Count() == 0)
            {
                return [possessionChanges];
            }

            List<PossessionChangeDTO> toPlace = [];

            foreach (Fumble fumble in play.Fumbles)
            {
                if (fumble.FumbleCommittedById != fumble.FumbleRecoveredById)
                {
                    possessionChanges.Add(new()
                    {
                        FromPlayerId = fumble.FumbleCommittedById ?? 0,
                        ToPlayerId = fumble.FumbleRecoveredById ?? 0,
                        BallReceivedAt = fumble.RecoveredAt,
                        EntityType = typeof(Fumble),
                        EntityId = fumble.Id,
                    });
                }
            };

            foreach (Lateral lateral in play.Laterals)
            {
                possessionChanges.Add(new()
                {
                    FromPlayerId = lateral.PrevCarrierId ?? 0,
                    ToPlayerId = lateral.NewCarrierId ?? 0,
                    BallReceivedAt = lateral.PossessionAt,
                    BallCarriedTo = lateral.CarriedTo,
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
                    int afterPlayerCompare = possessionChanges[possessionChanges.Count() - 1].ToPlayerId;
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

            bool fitFound = checkPlacementLayer(1);

            if (fitFound)
            {
                return paths
                    .Where((path) => path.Count() == toPlace.Count())
                    .Select((path) =>
                    {
                        List<PossessionChangeDTO> newChain = possessionChanges;
                        foreach (int index in path)
                        {
                            PossessionChangeDTO addChange = toPlace[Math.Abs(index) - 1];
                            if (index > 0)
                            {
                                newChain.Add(addChange);
                            }
                            else
                            {
                                newChain = newChain.Skip(1).ToList();
                                newChain.Insert(0, addChange);
                                newChain.Insert(0, new()
                                {
                                    ToPlayerId = addChange.FromPlayerId,
                                    EntityType = addChange.EntityType,
                                    EntityId = addChange.EntityId,
                                });
                            }
                        }
                        return newChain;
                    })
                    .ToList();
            }

            return [[]];
        }
    }
}
