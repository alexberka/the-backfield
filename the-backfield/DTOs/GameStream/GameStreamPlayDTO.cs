using TheBackfield.Models;

namespace TheBackfield.DTOs.GameStream
{
    public class GameStreamPlayDTO
    {
        private readonly Play _play;
        public GameStreamPlayDTO(Play play)
        {
            _play = play;
        }

        public int Id { get { return _play.Id; } }
        public int? TeamId { get { return _play.TeamId; } }
        public int? FieldPositionStart { get { return _play.FieldPositionStart; } }
        public int? FieldPositionEnd { get { return _play.FieldPositionEnd; } }
        public int Down { get { return _play.Down; } }
        public int? ToGain { get { return _play.ToGain; } }
        public int? ClockStart { get { return _play.ClockStart; } }
        public int? ClockEnd { get { return _play.ClockEnd; } }
        public int? GamePeriod { get { return _play.GamePeriod; } }
        public string Notes { get { return _play.Notes; } }
        public object? Pass
        {
            get
            {
                if (_play.Pass == null)
                {
                    return null;
                }

                return new
                {
                    _play.Pass.Passer,
                    _play.Pass.Receiver,
                    _play.Pass.Completion
                };
            }
        }
        public object? Rush
        {
            get
            {
                if (_play.Rush == null)
                {
                    return null;
                }

                return new
                {
                    _play.Rush.Rusher,
                };
            }
        }
        public List<Player?> Tacklers
        {
            get
            {
                if (_play.Tacklers.Count == 0)
                {
                    return [];
                }

                return _play.Tacklers.Select(t => t.Tackler).ToList();
            }
        }
        public List<Player?> PassDefenders
        {
            get
            {
                if (_play.PassDefenders.Count == 0)
                {
                    return [];
                }

                return _play.PassDefenders.Select(t => t.Defender).ToList();
            }
        }
        public object? Kickoff
        {
            get
            {
                if (_play.Kickoff == null)
                {
                    return null;
                }

                return new
                {
                    _play.Kickoff.Kicker,
                    _play.Kickoff.Returner,
                    _play.Kickoff.FieldedAt,
                    _play.Kickoff.Touchback,
                };
            }
        }
        public object? Punt
        {
            get
            {
                if (_play.Punt == null)
                {
                    return null;
                }

                return new
                {
                    _play.Punt.Kicker,
                    _play.Punt.Returner,
                    _play.Punt.FieldedAt,
                    _play.Punt.FairCatch,
                    _play.Punt.Touchback,
                    _play.Punt.Fake,
                };
            }
        }
        public object? FieldGoal
        {
            get
            {
                if (_play.FieldGoal == null)
                {
                    return null;
                }

                return new
                {
                    _play.FieldGoal.Kicker,
                    _play.FieldGoal.Good,
                    _play.FieldGoal.Fake,
                };
            }
        }
        public object? Touchdown
        {
            get
            {
                if (_play.Touchdown == null)
                {
                    return null;
                }

                return new
                {
                    _play.Touchdown.Player
                };
            }
        }
        public object? ExtraPoint
        {
            get
            {
                if (_play.ExtraPoint == null)
                {
                    return null;
                }

                return new
                {
                    _play.ExtraPoint.Kicker,
                    _play.ExtraPoint.Good,
                    _play.ExtraPoint.Fake,
                    _play.ExtraPoint.DefensiveConversion,
                    _play.ExtraPoint.Returner,
                };
            }
        }
        public object? Conversion
        {
            get
            {
                if (_play.Conversion == null)
                {
                    return null;
                }

                return new
                {
                    _play.Conversion.Passer,
                    _play.Conversion.Receiver,
                    _play.Conversion.Rusher,
                    _play.Conversion.Good,
                    _play.Conversion.DefensiveConversion,
                    _play.Conversion.Returner,
                };
            }
        }
        public object? Safety
        {
            get
            {
                if (_play.Safety == null)
                {
                    return null;
                }

                return new
                {
                    _play.Safety.CedingPlayer
                };
            }
        }
        public List<object> Fumbles
        {
            get
            {
                if (_play.Fumbles.Count == 0)
                {
                    return [];
                }

                List<object> fumbles = (List<object>)_play.Fumbles.Select(f => new
                {
                    f.FumbleCommittedBy,
                    f.FumbledAt,
                    f.FumbleForcedBy,
                    f.FumbleRecoveredBy,
                    f.RecoveredAt
                });

                return fumbles;
            }
        }
        public object? Interception
        {
            get
            {
                if (_play.Interception == null)
                {
                    return null;
                }

                return new
                {
                    _play.Interception.InterceptedBy,
                    _play.Interception.InterceptedAt
                };
            }
        }
        public object? KickBlock
        {
            get
            {
                if (_play.KickBlock == null)
                {
                    return null;
                }

                return new
                {
                    _play.KickBlock.BlockedBy,
                    _play.KickBlock.RecoveredBy,
                    _play.KickBlock.RecoveredAt,
                };
            }
        }
        public List<object> Laterals
        {
            get
            {
                if (_play.Laterals.Count == 0)
                {
                    return [];
                }

                List<object> laterals = (List<object>)_play.Laterals.Select(l => new
                {
                    l.PrevCarrier,
                    l.NewCarrier,
                    l.PossessionAt,
                    l.CarriedTo,
                });

                return laterals;
            }
        }
        public List<GameStreamPlayPenaltyDTO> Penalties
        {
            get
            {
                if (_play.Penalties.Count == 0)
                {
                    return [];
                }

                return _play.Penalties.Select(p => new GameStreamPlayPenaltyDTO(p)).ToList();
            }
        }
    }
}