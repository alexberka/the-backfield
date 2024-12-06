using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class PlayPenalty
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        [Required]
        public int PenaltyId { get; set; }
        public Penalty Penalty { get; set; }
        public int? PlayerId { get; set; }
        public Player? Player { get; set; }
        [Required]
        public int TeamId { get; set; }
        public bool Enforced { get; set; } = true;
        public int EnforcedFrom { get; set; }
        public bool NoPlay { get; set; }
        public bool AutoFirstDown { get; set; }
        public int Yardage { get; set; } // Penalty yardage provided as positive value
    }
}
