using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Punt
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play? Play { get; set; }
        public int? KickerId { get; set; } = null;
        public Player? Kicker { get; set; }
        public int TeamId { get; set; }
        public int? ReturnerId { get; set; } = null;
        public Player? Returner { get; set; }
        public int ReturnTeamId { get; set; }
        public int? FieldedAt { get; set; } = null;
        public bool FairCatch { get; set; }
        public bool Touchback { get; set; }
        public bool Fake { get; set; }
        public int Distance { get; set; }
        public int ReturnYardage { get; set; }
    }
}
