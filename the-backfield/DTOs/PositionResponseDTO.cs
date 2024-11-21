using TheBackfield.Models;

namespace TheBackfield.DTOs
{
    public class PositionResponseDTO : ResponseDTO
    {
        public Position? Position { get; set; }
        public List<Position> Positions { get; set; } = [];
    }
}
