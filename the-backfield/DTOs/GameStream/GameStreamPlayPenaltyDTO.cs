using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.DTOs.GameStream
{
    public class GameStreamPlayPenaltyDTO
    {
        private readonly PlayPenalty _playPenalty;

        public GameStreamPlayPenaltyDTO(PlayPenalty playPenalty)
        {
            _playPenalty = playPenalty;
        }

        public string? Name { get { return _playPenalty.Penalty?.Name; } }
        public Player? Player { get { return _playPenalty.Player; } }
        public int TeamId { get { return _playPenalty.TeamId; } }
        public bool Enforced { get { return _playPenalty.Enforced; } }
        public int EnforcedFrom { get { return _playPenalty.EnforcedFrom; } }
        public bool NoPlay { get { return _playPenalty.NoPlay; } }
        public bool LossOfDown { get { return _playPenalty.LossOfDown; } }
        public bool AutoFirstDown { get { return _playPenalty.AutoFirstDown; } }
        public int Yardage { get { return _playPenalty.Yardage; } }
    }
}
