using TheBackfield.Models;

namespace TheBackfield.DTOs
{
    public class PenaltyResponseDTO : ResponseDTO
    {
        public Penalty? Penalty { get; set; }
        public List<Penalty> Penalties { get; set; } = [];
    }
}