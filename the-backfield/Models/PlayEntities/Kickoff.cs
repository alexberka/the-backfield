using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Kickoff
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        public int? KickerId { get; set; } = null;
        public Player? Kicker { get; set; }
        public int? ReturnerId { get; set; }
        public Player? Returner { get; set; }
        public int FieldedAt { get; set; }
        public bool Touchback { get; set; }
    }
}
