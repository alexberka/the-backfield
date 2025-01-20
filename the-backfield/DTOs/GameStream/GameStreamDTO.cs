using TheBackfield.Models;

namespace TheBackfield.DTOs.GameStream
{
    public class GameStreamDTO
    {
        private readonly Game _game;
        private readonly PlaySubmitDTO _nextPlay;
        public GameStreamDTO(Game game, PlaySubmitDTO nextPlay)
        {
            _game = game;
            _nextPlay = nextPlay;
        }
        public Team? HomeTeam { get { return _game.HomeTeam; } }
        public List<PlayerStatsDTO> HomeTeamPlayerStats { get; set; } = [];
        public int HomeTeamScore { get; set; }
        public Team? AwayTeam { get { return _game.AwayTeam; } }
        public List<PlayerStatsDTO> AwayTeamPlayerStats { get; set; } = [];
        public int AwayTeamScore { get; set; }
        public int DrivePlays { get; set; }
        public int? DrivePositionStart { get; set; }
        public int DriveYards { get; set; }
        public int DriveTime { get; set; }
        public PlayAsSegmentsDTO? LastPlay { get; set; }
        public PlaySubmitDTO NextPlay { get { return _nextPlay; } }
    }
}
