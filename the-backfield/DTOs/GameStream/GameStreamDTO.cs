using TheBackfield.Models;
using TheBackfield.Utilities;

namespace TheBackfield.DTOs.GameStream
{
    public class GameStreamDTO
    {
        private readonly Game _game;
        private readonly Play? _currentPlay;
        private readonly int? _nextTeamId;
        private readonly int? _lastFieldPositionEnd = null;
        private readonly int _down;
        private readonly int? _toGain;
        public GameStreamDTO(Game game)
        {
            _game = game;
            _currentPlay = game.Plays.SingleOrDefault(p => !_game.Plays.Any(gp => gp.PrevPlayId == p.Id));
            var (homeTeamScore, awayTeamScore) = StatClient.ParseScore(game);

            if (_currentPlay != null)
            {
                var (down, toGain, fieldPositionEnd, nextTeamId) = StatClient.ParseFieldPosition(_currentPlay, game.HomeTeamId, game.AwayTeamId);
                _down = down;
                _toGain = toGain;
                _lastFieldPositionEnd = fieldPositionEnd;
                _nextTeamId = nextTeamId;
            }
            else
            {
                _down = 0;
                _toGain = null;
                _lastFieldPositionEnd = null;
                _nextTeamId = null;
            }
        }
        public Team? HomeTeam { get { return _game.HomeTeam; } }
        public Team? AwayTeam { get { return _game.AwayTeam; } }
        public PlaySubmitDTO NextPlay
        {
            get
            {
                int? clockStart = null;
                int? gamePeriod = null;
                int playCheckId = _currentPlay?.Id ?? 0;
                do
                {
                    Play? checkPlay = _game.Plays.SingleOrDefault(p => p.Id == playCheckId);
                    if (checkPlay == null)
                    {
                        clockStart = _game.PeriodLength;
                        gamePeriod = 1;
                    }
                    else
                    {
                        if (gamePeriod == null && checkPlay.GamePeriod != null)
                        {
                            gamePeriod = checkPlay.GamePeriod;
                        }
                        if (checkPlay.ClockEnd != null)
                        {
                            clockStart = checkPlay.ClockEnd;
                        }
                        else if (checkPlay.ClockStart != null)
                        {
                            clockStart = checkPlay.ClockStart;
                        }
                        else if (gamePeriod != null && (checkPlay.GamePeriod ?? gamePeriod) != gamePeriod)
                        {
                            clockStart = _game.PeriodLength;
                        }
                        else
                        {
                            playCheckId = checkPlay.PrevPlayId ?? 0;
                        }
                    }
                } while (clockStart == null || gamePeriod == null);

                if (clockStart == 0)
                {
                    gamePeriod += 1;
                    clockStart = _game.PeriodLength;
                }

                return new PlaySubmitDTO
                {
                    PrevPlayId = _currentPlay?.Id ?? -1,
                    GameId = _game.Id,
                    TeamId = _nextTeamId ?? 0,
                    FieldPositionStart = _lastFieldPositionEnd,
                    Down = _down,
                    ToGain = _toGain,
                    ClockStart = clockStart,
                    GamePeriod = gamePeriod
                };
            }
        }
    }
}
