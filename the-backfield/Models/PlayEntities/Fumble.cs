using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Fumble
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        public int? FumbleCommittedById { get; set; } = null;
        public Player FumbleCommittedBy { get; set; }
        public int? FumbledAt { get; set; }
        public int? FumbleForcedById { get; set; } = null;
        public Player? FumbleForcedBy { get; set; }
        public int? FumbleRecoveredById { get; set; } = null;
        public Player? FumbleRecoveredBy { get; set; }
        public int? RecoveredAt { get; set; }
        public int LooseBallYardage { get; set; }
        public int ReturnYardage { get; set; }
        public string YardageType { get; set; } = "";
    }
}
