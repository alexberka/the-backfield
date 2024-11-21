using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.DTOs
{
    public class TeamResponseDTO : ResponseDTO
    {
        public Team? Team { get; set; }
        public int TeamId { get; set; }
        public List<Team> Teams { get; set; } = [];
    }
}
