using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.DTOs
{
    public class GameResponseDTO : ResponseDTO
    {
        public IGame? Game { get; set; }
        public IEnumerable<IGame> Games { get; set; } = [];
    }
}
