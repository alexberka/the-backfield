using System.Text.Json.Serialization;
using the_backfield.Migrations;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.DTOs
{
    public class GameStreamDTO
    {
        private readonly Game _game;
        private readonly Play? _currentPlay;
        public GameStreamDTO(Game game)
        {
            _game = game;
            _currentPlay = game.Plays.SingleOrDefault(p => !_game.Plays.Any(gp => gp.PrevPlayId == p.Id));
        }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public int DriveFieldPositionStart { get; set; }
        public int DriveNumberOfPlays { get; set; }
        public int DriveLength { get; set; }
        public GameStreamPlayDTO? PrevPlay
        {
            get
            {
                if (_currentPlay == null)
                {
                    return null;
                }
                Play? prevPlay = _game.Plays.SingleOrDefault(p => p.Id == _currentPlay.PrevPlayId);
                if (prevPlay == null)
                {
                    return null;
                }

                return new GameStreamPlayDTO(prevPlay);
            } 
        }
        public object? CurrentPlay
        {
            get
            {
                if (_currentPlay == null)
                {
                    return null;
                }

                return new GameStreamPlayDTO(_currentPlay);
            }
        }
        public PlaySubmitDTO NextPlay
        {
            get
            {
                return new PlaySubmitDTO
                {
                    PrevPlayId = _currentPlay?.Id ?? -1,
                    GameId = _game.Id
                };
            }
        }
    }
}
