using the_backfield.Migrations;
using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    }
}
