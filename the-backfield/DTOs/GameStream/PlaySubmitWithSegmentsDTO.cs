using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Utilities;

namespace TheBackfield.DTOs.GameStream
{
    public class PlaySubmitWithSegmentsDTO
    {
        private readonly PlaySubmitDTO _playSubmit;
        private List<PlaySegmentDTO> _playSegments;
        public PlaySubmitWithSegmentsDTO(Play play, List<PlaySegmentDTO> playSegments)
        {
            _playSubmit = PlayClient.PlayAsPlaySubmitDTO(play);
            _playSegments = playSegments ?? [];
        }
        public PlaySubmitWithSegmentsDTO(PlaySubmitDTO playSubmit, List<PlaySegmentDTO> playSegments)
        {
            _playSubmit = playSubmit;
            _playSegments = playSegments ?? [];
        }
        public int Id { get { return _playSubmit.Id; } }
        public int? PrevPlayId { get { return _playSubmit.PrevPlayId; } }
        public int? GameId { get { return _playSubmit.GameId; } }
        public int? TeamId { get { return _playSubmit.TeamId; } }
        public int? FieldPositionStart { get { return _playSubmit.FieldPositionStart; } }
        public int? FieldPositionEnd { get { return _playSubmit.FieldPositionEnd; } }
        public int? Down { get { return _playSubmit.Down; } }
        public int? ToGain { get { return _playSubmit.ToGain; } }
        public int? ClockStart { get { return _playSubmit.ClockStart; } }
        public int? ClockEnd { get { return _playSubmit.ClockEnd; } }
        public int? GamePeriod { get { return _playSubmit.GamePeriod; } }
        public string Notes { get { return _playSubmit.Notes; } }
        public int? PasserId { get { return _playSubmit.PasserId; } }
        public int? ReceiverId { get { return _playSubmit.ReceiverId; } }
        public bool Completion { get { return _playSubmit.Completion; } }
        public int? RusherId { get { return _playSubmit.RusherId; } }
        public List<int> TacklerIds { get { return _playSubmit.TacklerIds; } }
        public List<int> PassDefenderIds { get { return _playSubmit.PassDefenderIds; } }
        public bool Kickoff { get { return _playSubmit.Kickoff; } }
        public bool Punt { get { return _playSubmit.Punt; } }
        public bool FieldGoal { get { return _playSubmit.FieldGoal; } }
        public int? KickerId { get { return _playSubmit.KickerId; } }
        public int? KickReturnerId { get { return _playSubmit.KickReturnerId; } }
        public int? KickFieldedAt { get { return _playSubmit.KickFieldedAt; } }
        public bool KickFairCatch { get { return _playSubmit.KickFairCatch; } }
        public bool KickGood { get { return _playSubmit.KickGood; } }
        public bool KickTouchback { get { return _playSubmit.KickTouchback; } }
        public bool KickFake { get { return _playSubmit.KickFake; } }
        public int? TouchdownPlayerId { get { return _playSubmit.TouchdownPlayerId; } }
        public bool ExtraPoint { get { return _playSubmit.ExtraPoint; } }
        public bool Conversion { get { return _playSubmit.Conversion; } }
        public int? ExtraPointKickerId { get { return _playSubmit.ExtraPointKickerId; } }
        public bool ExtraPointGood { get { return _playSubmit.ExtraPointGood; } }
        public bool ExtraPointFake { get { return _playSubmit.ExtraPointFake; } }
        public int? ConversionPasserId { get { return _playSubmit.ConversionPasserId; } }
        public int? ConversionReceiverId { get { return _playSubmit.ConversionReceiverId; } }
        public int? ConversionRusherId { get { return _playSubmit.ConversionRusherId; } }
        public bool ConversionGood { get { return _playSubmit.ConversionGood; } }
        public bool DefensiveConversion { get { return _playSubmit.DefensiveConversion; } }
        public int? ConversionReturnerId { get { return _playSubmit.ConversionReturnerId; } }
        public bool Safety { get { return _playSubmit.Safety; } }
        public int? CedingPlayerId { get { return _playSubmit.CedingPlayerId; } }
        public List<FumbleSubmitDTO> Fumbles { get { return _playSubmit.Fumbles; } }
        public int? InterceptedById { get { return _playSubmit.InterceptedById; } }
        public int? InterceptedAt { get { return _playSubmit.InterceptedAt; } }
        public bool KickBlocked { get { return _playSubmit.KickBlocked; } }
        public int? KickBlockedById { get { return _playSubmit.KickBlockedById; } }
        public int? KickBlockRecoveredById { get { return _playSubmit.KickBlockRecoveredById; } }
        public int? KickBlockRecoveredAt { get { return _playSubmit.KickBlockRecoveredAt; } }
        public List<LateralSubmitDTO> Laterals { get { return _playSubmit.Laterals; } }
        public List<PlayPenaltySubmitDTO> Penalties { get { return _playSubmit.Penalties; } }
        public List<PlaySegmentDTO> PlaySegments
        {
            get { return _playSegments; } 
            set { _playSegments = value; }
        }
    }
}
