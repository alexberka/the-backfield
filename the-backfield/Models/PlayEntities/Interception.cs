using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Interception
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        [Required]
        public int InterceptedById { get; set; }
        public Player InterceptedBy { get; set; }
        public int InterceptedAt { get; set; }
    }
}
