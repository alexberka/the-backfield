using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Interception
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        public int? InterceptedById { get; set; } = null;
        public Player InterceptedBy { get; set; }
        public int? InterceptedAt { get; set; }
    }
}
