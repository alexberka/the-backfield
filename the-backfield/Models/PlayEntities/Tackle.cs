using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Tackle
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        public int? TacklerId { get; set; } = null;
        public Player? Tackler { get; set; }
    }
}
