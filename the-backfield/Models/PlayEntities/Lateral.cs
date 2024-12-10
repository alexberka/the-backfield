using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Lateral
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        public int? NewCarrierId { get; set; } = null;
        public Player NewCarrier { get; set; }
        public int? PossessionAt { get; set; }
        public int? CarriedTo { get; set; }
    }
}
