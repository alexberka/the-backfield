using System.ComponentModel.DataAnnotations;

namespace TheBackfield.DTOs
{
    public class PlayerPositionSubmitDTO
    {
        [Required]
        public string? SessionKey { get; set; }
        [Required]
        public int PlayerId { get; set; }
        [Required]
        public List<int> PositionIds { get; set; } = []; 
    }
}
