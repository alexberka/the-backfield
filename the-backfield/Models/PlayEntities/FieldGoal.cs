using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class FieldGoal
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play? Play { get; set; }
        public int? KickerId { get; set; } = null;
        public Player? Kicker { get; set; }
        public bool Good { get; set; }
        public bool Fake { get; set; }
        public int Distance { get; set; }
    }
}
