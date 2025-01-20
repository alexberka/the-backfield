using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Touchdown
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play? Play { get; set; }
        public int? PlayerId { get; set; } = null;
        public Player? Player { get; set; }
        public int TeamId { get; set; }
    }
}