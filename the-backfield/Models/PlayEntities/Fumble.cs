using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Fumble
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        [Required]
        public int FumbleCommittedById { get; set; }
        public Player FumbleCommittedBy { get; set; }
        public int FumbledAt { get; set; }
        public int? FumbleForcedById { get; set; }
        public Player? FumbleForcedBy { get; set; }
        public int? FumbleRecoveredById { get; set; }
        public Player? FumbleRecoveredBy { get; set; }
        public int RecoveredAt { get; set; }
    }
}
