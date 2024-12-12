using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Safety
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play? Play { get; set; }
        public int? CedingPlayerId { get; set; } = null;
        public Player? CedingPlayer { get; set; }
    }
}
