using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Rush
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play? Play { get; set; }
        public int? RusherId { get; set; } = null;
        public Player? Rusher { get; set; }
        public int TeamId { get; set; }
        public int Yardage { get; set; }
    }
}
