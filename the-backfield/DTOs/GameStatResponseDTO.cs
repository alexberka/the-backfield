using TheBackfield.Models;

namespace TheBackfield.DTOs
{
    public class GameStatResponseDTO : ResponseDTO
    {
        public GameStat? GameStat { get; set; }
        public List<GameStat> GameStats { get; set; } = [];
    }
}
