using System.ComponentModel.DataAnnotations;
using TheBackfield.DTOs.PlayEntities;

namespace TheBackfield.DTOs
{
    public class PlaySubmitDTO
    {
        public int Id { get; set; }
        [Required]
        public int PrevPlayId { get; set; } // Reserve 1 for Game Start
        [Required]
        public int GameId { get; set; }
        [Required]
        public int TeamId { get; set; }
        public int? FieldPositionStart { get; set; } = null;
        public int? FieldPositionEnd { get; set; } = null;
        public int Down { get; set; } = 0;
        public int? ToGain { get; set; } = null;
        public int? ClockStart { get; set; } = null;
        public int? ClockEnd { get; set; } = null;
        public int? GamePeriod { get; set; } = null;
        public string Notes { get; set; } = "";
        public int? PasserId { get; set; } = null;
        public int? ReceiverId { get; set; } = null;
        public bool Completion { get; set; } = false;
        public int? RusherId { get; set; } = null;
        public List<int> TacklerIds { get; set; } = [];
        public List<int> PassDefenderIds { get; set; } = [];
        public bool Kickoff { get; set; } = false;
        public bool Punt { get; set; } = false;
        public bool FieldGoal { get; set; } = false;
        public int? KickerId { get; set; } = null;
        public int? KickReturnerId { get; set; } = null;
        public int? KickFieldedAt { get; set; } = null;
        public bool KickFairCatch { get; set; } = false;
        public bool KickGood { get; set; } = false;
        public bool KickTouchback { get; set; } = false;
        public bool KickFake { get; set; } = false;
        public bool ExtraPoint { get; set; } = false;
        public bool Conversion { get; set; } = false;
        public int? ExtraPointKickerId { get; set; } = null;
        public bool ExtraPointGood { get; set; } = false;
        public bool ExtraPointFake { get; set; } = false;
        public int? ConversionPasserId { get; set; } = null;
        public int? ConversionReceiverId { get; set; } = null;
        public int? ConversionRusherId { get; set; } = null;
        public bool ConversionGood { get; set; } = false;
        public bool DefensiveConversion { get; set; } = false;
        public int? ConversionReturnerId { get; set; } = null;
        public List<FumbleSubmitDTO> Fumbles { get; set; } = [];
        public int? InterceptedById { get; set; } = null;
        public int? InterceptedAt { get; set; } = null;
        public int? KickBlockedById { get; set; } = null;
        public int? KickBlockRecoveredById { get; set; } = null;
        public int? KickBlockReturnedTo { get; set; } = null;
        public List<LateralSubmitDTO> Laterals { get; set; } = [];
        public List<PlayPenaltySubmitDTO> Penalties { get; set; } = [];
        public string SessionKey { get; set; } = "";
    }
}
