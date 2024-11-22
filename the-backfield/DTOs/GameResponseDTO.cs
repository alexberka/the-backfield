using TheBackfield.Models;

namespace TheBackfield.DTOs
{
    public class GameResponseDTO : ResponseDTO
    {
        public Game? Game { get; set; }
        public List<Game> Games { get; set; } = [];
    }
}
