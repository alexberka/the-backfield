using TheBackfield.Models;

namespace TheBackfield.DTOs
{
    public class PlayerResponseDTO : ResponseDTO
    {
        public Player? Player { get; set; }
        public List<Player> Players { get; set; } = [];
    }
}
