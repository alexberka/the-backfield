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

        public static (int nextDown, int? nextToGain, int? thisFieldPosition, int? nextTeamId) ParseFieldPosition(Play play, int homeTeamId, int awayTeamId)
        {
            int? fieldPosition = play.FieldPositionEnd;
            List<PlayPenalty> enforcedPenalties = play.Penalties.Where(pe => pe.Enforced == true).ToList();
            if (enforcedPenalties.Count() > 0)
            {
                fieldPosition = enforcedPenalties[0].EnforcedFrom;
                foreach (PlayPenalty penalty in enforcedPenalties)
                {
                    int penaltyTeamSign = penalty.TeamId == homeTeamId ? -1 : 1;
                    fieldPosition += penaltyTeamSign * penalty.Yardage;
                    if (Math.Abs(fieldPosition ?? 0) > 50)
                    {
                        fieldPosition = penaltyTeamSign * 49;
                    }
                }
            }

            int teamId = play.TeamId ?? 0;
            int down = play.Down;
            int? toGain = play.ToGain;

            Dictionary<int, int> teamSigns = new()
            {
                { homeTeamId, 1 },
                { awayTeamId, -1 }
            };

            if (enforcedPenalties.Any(ep => ep.NoPlay == true) && !enforcedPenalties.Any(ep => ep.LossOfDown == true))
            {
                if (play.Kickoff == null && (enforcedPenalties.Any(ep => ep.AutoFirstDown == true) || (fieldPosition - toGain) * teamSigns[teamId] >= 0))
                {
                    down = 1;
                    toGain = Math.Abs((fieldPosition + (teamSigns[teamId] * 10)) ?? 0) > 50 ? teamSigns[teamId] * 50 : fieldPosition + (teamSigns[teamId] * 10);
                }
            }
            else if (play.Safety != null || play.Touchdown != null || play.FieldGoal?.Good == true)
            {
                down = 0;
                toGain = null;
                if (fieldPosition == 50)
                {
                    teamId = homeTeamId;
                }
                else if (fieldPosition == -50)
                {
                    teamId = awayTeamId;
                }
            }
            else
            {
                if (play.Kickoff != null || (play.Punt != null && !play.Punt.Fake))
                {
                    teamId = teamId == homeTeamId ? awayTeamId : homeTeamId;
                }
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
                if (teamId != play.TeamId && teamId != 0)
                {
                    down = 1;
                    toGain = Math.Abs(fieldPosition + (teamSigns[teamId] * 10) ?? 0) > 50 ? teamSigns[teamId] * 50 : fieldPosition + (teamSigns[teamId] * 10);
                }
                else
                {
                    if ((fieldPosition - toGain) * teamSigns[teamId] > 0)
                    {
                        down = 1;
                        toGain = Math.Abs(fieldPosition + (teamSigns[teamId] * 10) ?? 0) > 50 ? teamSigns[teamId] * 50 : fieldPosition + (teamSigns[teamId] * 10);
                    }
                    else if (play.Down < 4)
                    {
                        down = play.Down + 1;
                    }
                    else
                    {
                        teamId = teamId == homeTeamId ? awayTeamId : homeTeamId;
                        down = 1;
                        toGain = Math.Abs(fieldPosition + (teamSigns[teamId] * 10) ?? 0) > 50 ? teamSigns[teamId] * 50 : fieldPosition + (teamSigns[teamId] * 10);
                    }
                }
            }
            
            return (down, toGain, fieldPosition, teamId);
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
