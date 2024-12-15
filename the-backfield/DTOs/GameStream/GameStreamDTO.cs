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
        public Team? AwayTeam { get { return _game.AwayTeam; } }
        public PlaySubmitDTO NextPlay { get { return _nextPlay; } }
    }
}
