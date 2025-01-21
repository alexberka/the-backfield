using TheBackfield.DTOs.PlayEntities;
using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;
using TheBackfield.Models;

namespace TheBackfield.Utilities
{
    public class PlayClient
    {
        public static Play PlaySubmitDTOAsPlay(PlaySubmitDTO playSubmit)
        {
            Play newPlay = new()
            {
                Id = playSubmit.Id,
                PrevPlayId = playSubmit.PrevPlayId,
                GameId = playSubmit.GameId,
                TeamId = playSubmit.TeamId,
                FieldPositionStart = playSubmit.FieldPositionStart,
                FieldPositionEnd = playSubmit.FieldPositionEnd,
                Down = playSubmit.Down,
                ToGain = playSubmit.ToGain,
                ClockStart = playSubmit.ClockStart,
                ClockEnd = playSubmit.ClockEnd,
                GamePeriod = playSubmit.GamePeriod,
                Notes = playSubmit.Notes
            };

            // Add auxiliary entities
            // Create a Pass, if PasserId is defined
            if (playSubmit.PasserId != null)
            {
                newPlay.Pass = new()
                {
                    PlayId = newPlay.Id,
                    PasserId = playSubmit.PasserId,
                    ReceiverId = playSubmit.ReceiverId,
                    Completion = playSubmit.Completion
                };
            }

            // Create a Rush, if RusherId is defined
            if (playSubmit.RusherId != null)
            {
                newPlay.Rush = new()
                {
                    PlayId = newPlay.Id,
                    RusherId = playSubmit.RusherId
                };
            }

            // Create Tackles
            foreach (int tacklerId in playSubmit.TacklerIds)
            {
                if (!newPlay.Tacklers.Any(t => t.TacklerId == tacklerId))
                {
                    Tackle newTackle = new()
                    {
                        PlayId = newPlay.Id,
                        TacklerId = tacklerId
                    };
                    newPlay.Tacklers.Add(newTackle);
                }
            }

            // Create PassDefenses
            foreach (int defenderId in playSubmit.PassDefenderIds)
            {
                if (!newPlay.PassDefenders.Any(pd => pd.DefenderId == defenderId))
                {
                    PassDefense newPassDefense = new()
                    {
                        PlayId = newPlay.Id,
                        DefenderId = defenderId
                    };
                    newPlay.PassDefenders.Add(newPassDefense);
                }
            }

            // Create Kickoff, Punt, or FieldGoal
            if (playSubmit.Kickoff)
            {
                newPlay.Kickoff = new()
                {
                    PlayId = newPlay.Id,
                    KickerId = playSubmit.KickerId,
                    ReturnerId = playSubmit.KickReturnerId,
                    FieldedAt = playSubmit.KickFieldedAt,
                    Touchback = playSubmit.KickTouchback
                };
            }
            else if (playSubmit.Punt)
            {
                newPlay.Punt = new()
                {
                    PlayId = newPlay.Id,
                    KickerId = playSubmit.KickerId,
                    ReturnerId = playSubmit.KickReturnerId,
                    FieldedAt = playSubmit.KickFieldedAt,
                    FairCatch = playSubmit.KickFairCatch,
                    Touchback = playSubmit.KickTouchback,
                    Fake = playSubmit.KickFake
                };
            }
            else if (playSubmit.FieldGoal)
            {
                newPlay.FieldGoal = new()
                {
                    PlayId = newPlay.Id,
                    KickerId = playSubmit.KickerId,
                    Good = playSubmit.KickGood,
                    Fake = playSubmit.KickFake
                };
            }

            // Create KickBlock
            if (playSubmit.KickBlocked)
            {
                newPlay.KickBlock = new()
                {
                    PlayId = newPlay.Id,
                    BlockedById = playSubmit.KickBlockedById,
                    RecoveredById = playSubmit.KickBlockRecoveredById,
                    RecoveredAt = playSubmit.KickBlockRecoveredAt
                };
            }

            // Create Touchdown
            if (playSubmit.TouchdownPlayerId != null)
            {
                newPlay.Touchdown = new()
                {
                    PlayId = newPlay.Id,
                    PlayerId = playSubmit.TouchdownPlayerId
                };
            }

            // Create ExtraPoint
            if (playSubmit.ExtraPoint)
            {
                newPlay.ExtraPoint = new()
                {
                    PlayId = newPlay.Id,
                    KickerId = playSubmit.ExtraPointKickerId,
                    Good = playSubmit.ExtraPointGood,
                    Fake = playSubmit.ExtraPointFake,
                    DefensiveConversion = playSubmit.DefensiveConversion,
                    ReturnerId = playSubmit.ConversionReturnerId
                };
            }

            // Create Conversion
            if (playSubmit.Conversion)
            {
                newPlay.Conversion = new()
                {
                    PlayId = newPlay.Id,
                    PasserId = playSubmit.ConversionPasserId,
                    ReceiverId = playSubmit.ConversionReceiverId,
                    RusherId = playSubmit.ConversionRusherId,
                    Good = playSubmit.ConversionGood,
                    DefensiveConversion = playSubmit.DefensiveConversion,
                    ReturnerId = playSubmit.ConversionReturnerId
                };
            }

            // Create Interception
            if (playSubmit.InterceptedById != null)
            {
                newPlay.Interception = new()
                {
                    PlayId = newPlay.Id,
                    InterceptedById = playSubmit.InterceptedById,
                    InterceptedAt = playSubmit.InterceptedAt
                };
            }

            // Create Safety
            if (playSubmit.Safety)
            {
                Safety newSafety = new()
                {
                    PlayId = newPlay.Id,
                    CedingPlayerId = playSubmit.CedingPlayerId
                };
            }

            // Create Fumbles
            foreach (FumbleSubmitDTO fumbleSubmit in playSubmit.Fumbles)
            {
                Fumble newFumble = new()
                {
                    PlayId = newPlay.Id,
                    FumbleCommittedById = fumbleSubmit.FumbleCommittedById,
                    FumbledAt = fumbleSubmit.FumbledAt,
                    FumbleForcedById = fumbleSubmit.FumbleForcedById,
                    FumbleRecoveredById = fumbleSubmit.FumbleRecoveredById,
                    RecoveredAt = fumbleSubmit.FumbleRecoveredAt
                };
                newPlay.Fumbles.Add(newFumble);
            }

            // Create Laterals
            foreach (LateralSubmitDTO lateralSubmit in playSubmit.Laterals)
            {
                Lateral newLateral = new()
                {
                    PlayId = newPlay.Id,
                    PrevCarrierId = lateralSubmit.PrevCarrierId,
                    NewCarrierId = lateralSubmit.NewCarrierId,
                    PossessionAt = lateralSubmit.PossessionAt,
                    CarriedTo = lateralSubmit.CarriedTo
                };
                newPlay.Laterals.Add(newLateral);
            }

            // Create PlayPenalties
            foreach (PlayPenaltySubmitDTO playPenaltySubmit in playSubmit.Penalties)
            {
                PlayPenalty newPlayPenalty = new()
                {
                    PlayId = newPlay.Id,
                    PenaltyId = playPenaltySubmit.PenaltyId,
                    PlayerId = playPenaltySubmit.PlayerId,
                    TeamId = playPenaltySubmit.TeamId ?? 0,
                    Enforced = playPenaltySubmit.Enforced,
                    EnforcedFrom = playPenaltySubmit.EnforcedFrom,
                    NoPlay = playPenaltySubmit.NoPlay,
                    LossOfDown = playPenaltySubmit.LossOfDown,
                    AutoFirstDown = playPenaltySubmit.AutoFirstDown,
                    Yardage = playPenaltySubmit.Yardage ?? 0
                };
                newPlay.Penalties.Add(newPlayPenalty);
            }

            return newPlay;
        }

        public static PlaySubmitDTO PlayAsPlaySubmitDTO(Play play)
        {
            PlaySubmitDTO newPlaySubmit = new()
            {
                Id = play.Id,
                PrevPlayId = play.PrevPlayId ?? 0,
                GameId = play.GameId ?? 0,
                TeamId = play.TeamId ?? 0,
                FieldPositionStart = play.FieldPositionStart,
                FieldPositionEnd = play.FieldPositionEnd,
                Down = play.Down,
                ToGain = play.ToGain,
                ClockStart = play.ClockStart,
                ClockEnd = play.ClockEnd,
                GamePeriod = play.GamePeriod,
                Notes = play.Notes,
            };

            newPlaySubmit.PasserId = play.Pass?.PasserId;
            newPlaySubmit.ReceiverId = play.Pass?.ReceiverId;
            newPlaySubmit.Completion = play.Pass?.Completion ?? false;

            newPlaySubmit.RusherId = play.Rush?.RusherId;

            foreach (Tackle tackle in play.Tacklers)
            {
                if (tackle.TacklerId != null)
                {
                    newPlaySubmit.TacklerIds.Add((int)tackle.TacklerId);
                }
            }

            foreach (PassDefense passDefense in play.PassDefenders)
            {
                if (passDefense.DefenderId != null)
                {
                    newPlaySubmit.PassDefenderIds.Add((int)passDefense.DefenderId);
                }
            }

            if (play.Kickoff != null)
            {
                newPlaySubmit.Kickoff = true;
                newPlaySubmit.KickerId = play.Kickoff.KickerId;
                newPlaySubmit.KickReturnerId = play.Kickoff.ReturnerId;
                newPlaySubmit.KickFieldedAt = play.Kickoff.FieldedAt;
                newPlaySubmit.KickTouchback = play.Kickoff.Touchback;
            }
            if (play.Punt != null)
            {
                newPlaySubmit.Punt = true;
                newPlaySubmit.KickerId = play.Punt.KickerId;
                newPlaySubmit.KickReturnerId = play.Punt.ReturnerId;
                newPlaySubmit.KickFieldedAt = play.Punt.FieldedAt;
                newPlaySubmit.KickFairCatch = play.Punt.FairCatch;
                newPlaySubmit.KickTouchback = play.Punt.Touchback;
                newPlaySubmit.KickFake = play.Punt.Fake;
            }
            if (play.FieldGoal != null)
            {
                newPlaySubmit.FieldGoal = true;
                newPlaySubmit.KickerId = play.FieldGoal.KickerId;
                newPlaySubmit.KickGood = play.FieldGoal.Good;
                newPlaySubmit.KickFake = play.FieldGoal.Fake;
            }

            newPlaySubmit.TouchdownPlayerId = play.Touchdown?.PlayerId;

            if (play.ExtraPoint != null)
            {
                newPlaySubmit.ExtraPoint = true;
                newPlaySubmit.ExtraPointKickerId = play.ExtraPoint.KickerId;
                newPlaySubmit.ExtraPointGood = play.ExtraPoint.Good;
                newPlaySubmit.ExtraPointFake = play.ExtraPoint.Fake;
                newPlaySubmit.DefensiveConversion = play.ExtraPoint.DefensiveConversion;
                newPlaySubmit.ConversionReturnerId = play.ExtraPoint.ReturnerId;
            }
            if (play.Conversion != null)
            {
                newPlaySubmit.Conversion = true;
                newPlaySubmit.ConversionPasserId = play.Conversion.PasserId;
                newPlaySubmit.ConversionReceiverId = play.Conversion.ReceiverId;
                newPlaySubmit.ConversionRusherId = play.Conversion.RusherId;
                newPlaySubmit.ConversionGood = play.Conversion.Good;
                newPlaySubmit.DefensiveConversion = play.Conversion.DefensiveConversion;
                newPlaySubmit.ConversionReturnerId = play.Conversion.ReturnerId;
            }

            newPlaySubmit.Safety = play.Safety != null;
            newPlaySubmit.CedingPlayerId = play.Safety?.CedingPlayerId;

            foreach (Fumble fumble in play.Fumbles)
            {
                FumbleSubmitDTO newFumble = new()
                {
                    Id = fumble.Id,
                    PlayId = play.Id,
                    FumbleCommittedById = fumble.FumbleCommittedById ?? 0,
                    FumbledAt = fumble.FumbledAt,
                    FumbleForcedById = fumble.FumbleForcedById,
                    FumbleRecoveredById = fumble.FumbleRecoveredById,
                    FumbleRecoveredAt = fumble.RecoveredAt
                };
                newPlaySubmit.Fumbles.Add(newFumble);
            }

            newPlaySubmit.InterceptedById = play.Interception?.InterceptedById;
            newPlaySubmit.InterceptedAt = play.Interception?.InterceptedAt;

            newPlaySubmit.KickBlocked = play.KickBlock != null;
            newPlaySubmit.KickBlockedById = play.KickBlock?.BlockedById;
            newPlaySubmit.KickBlockRecoveredById = play.KickBlock?.RecoveredById;
            newPlaySubmit.KickBlockRecoveredAt = play.KickBlock?.RecoveredAt;

            foreach (Lateral lateral in play.Laterals)
            {
                LateralSubmitDTO newLateral = new()
                {
                    Id = lateral.Id,
                    PlayId = play.Id,
                    PrevCarrierId = lateral.PrevCarrierId ?? 0,
                    NewCarrierId = lateral.NewCarrierId ?? 0,
                    PossessionAt = lateral.PossessionAt,
                    CarriedTo = lateral.CarriedTo
                };
                newPlaySubmit.Laterals.Add(newLateral);
            }

            foreach (PlayPenalty playPenalty in play.Penalties)
            {
                PlayPenaltySubmitDTO newPlayPenalty = new()
                {
                    Id = playPenalty.Id,
                    PlayId = play.Id,
                    PenaltyId = playPenalty.PenaltyId,
                    PlayerId = playPenalty.PlayerId,
                    TeamId = playPenalty.TeamId,
                    Enforced = playPenalty.Enforced,
                    EnforcedFrom = playPenalty.EnforcedFrom,
                    NoPlay = playPenalty.NoPlay,
                    LossOfDown = playPenalty.LossOfDown,
                    AutoFirstDown = playPenalty.AutoFirstDown,
                    Yardage = playPenalty.Yardage
                };
                newPlaySubmit.Penalties.Add(newPlayPenalty);
            }

            return newPlaySubmit;
        }
    }
}
