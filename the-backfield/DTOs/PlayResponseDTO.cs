using TheBackfield.Models;

namespace TheBackfield.DTOs
{
    public class PlayResponseDTO : ResponseDTO
    {
        public Play? Play { get; set; }
        public List<Play> Plays { get; set; } = [];
    }
}
