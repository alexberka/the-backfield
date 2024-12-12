using TheBackfield.Models;

namespace TheBackfield.DTOs
{
    public class GameStreamDTO
    {
        public Game Game { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public int DriveFieldPositionStart { get; set; }
        public int DriveNumberOfPlays { get; set; }
        public int DriveLength { get; set; }
        public Play? PrevPlay { get; set; } = null;
        public Play? CurrentPlay { get; set; } = null;
        public PlaySubmitDTO NextPlay { get; set; } = new PlaySubmitDTO();
    }
}
