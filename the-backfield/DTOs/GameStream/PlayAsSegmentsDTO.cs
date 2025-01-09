using TheBackfield.Models;

namespace TheBackfield.DTOs.GameStream
{
    public class PlayAsSegmentsDTO
    {
        private readonly Play _play;
        private List<PlaySegmentDTO> _playSegments;
        public PlayAsSegmentsDTO(Play play, List<PlaySegmentDTO> playSegments)
        {
            _play = play;
            _playSegments = playSegments ?? [];
        }
        public int Id { get { return _play.Id; } }
        public int? PrevPlayId { get { return _play.PrevPlayId; } }
        public int? GameId { get { return _play.GameId; } }
        public int? TeamId { get { return _play.TeamId; } }
        public int? FieldPositionStart { get { return _play.FieldPositionStart; } }
        public int? FieldPositionEnd { get { return _play.FieldPositionEnd; } }
        public int Down { get { return _play.Down; } }
        public int? ToGain { get { return _play.ToGain; } }
        public int? ClockStart { get { return _play.ClockStart; } }
        public int? ClockEnd { get { return _play.ClockEnd; } }
        public int? GamePeriod { get { return _play.GamePeriod; } }
        public string Notes { get { return _play.Notes; } }
        public List<PlaySegmentDTO> PlaySegments
        {
            get { return _playSegments; } 
            set { _playSegments = value; }
        }
    }
}
