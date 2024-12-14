using System.Text.Json.Serialization;
using TheBackfield.Models;
using TheBackfield.Utilities;

namespace TheBackfield.DTOs.GameStream
{
    public class GameStreamDTO
    {
        private readonly Game _game;
        private readonly Play? _currentPlay;
        private readonly int _homeTeamScore;
        private readonly int _awayTeamScore;
        private readonly List<Play> _drive;
        private readonly int? _nextTeamId;
        private readonly int? _lastFieldPositionEnd = null;
        private readonly int _down;
        private readonly int? _toGain;
        public GameStreamDTO(Game game)
        {
            _game = game;
            _currentPlay = game.Plays.SingleOrDefault(p => !_game.Plays.Any(gp => gp.PrevPlayId == p.Id));
            var (homeTeamScore, awayTeamScore) = StatClient.ParseScore(game);
            _homeTeamScore = homeTeamScore;
            _awayTeamScore = awayTeamScore;

            _drive = [];

            if (_currentPlay != null)
            {
                var (down, toGain, fieldPositionEnd, nextTeamId) = StatClient.ParseFieldPosition(_currentPlay, game.HomeTeamId, game.AwayTeamId);
                _down = down;
                _toGain = toGain;
                _lastFieldPositionEnd = fieldPositionEnd;
                _nextTeamId = nextTeamId;

                bool driveFound = false;
                int playCheckId = _currentPlay.Id;
                do
                {
                    Play? playCheck = _game.Plays.SingleOrDefault(p => p.Id == playCheckId);
                    if (playCheck == null)
                    {
                        driveFound = true;
                    }
                    //else if (playCheck.TeamId != _nextTeamId && playCheck.Kickoff == null)
                    //{
                    //    driveFound = true;
                    //}
                    else
                    {
                        if (playCheck.Penalties.Any(pe => pe.NoPlay == true && pe.Enforced == true))
                        {
                            _drive.Add(playCheck);
                        }
                        else
                        {
                            if (playCheck.Kickoff != null)
                            {
                                _drive.Add(playCheck);
                                driveFound = true;
                            }
                            else if (_drive.Count > 0)
                            {
                                if (playCheck.Touchdown != null || playCheck.Safety != null
                                || playCheck.FieldGoal != null && playCheck.FieldGoal.Fake == false)
                                {
                                    driveFound = true;
                                }
                                else if (playCheck.Fumbles.Count > 0 || playCheck.KickBlock != null || playCheck.Interception != null)
                                {

                                }
                            }
                            if (!driveFound)
                            {
                                _drive.Add(playCheck);
                            }
                        }
                    }
                    if (!driveFound)
                    {
                        playCheckId = playCheck?.PrevPlayId ?? 0;
                    }
                } while (!driveFound);
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
        public int HomeTeamScore { get { return _homeTeamScore; } }
        public Team? AwayTeam { get { return _game.AwayTeam; } }
        public int AwayTeamScore { get { return _awayTeamScore; } }
        [JsonIgnore]
        public List<GameStreamPlayDTO> Drive
        {
            get
            {
                List<GameStreamPlayDTO> thisDrive = [];
                foreach (Play play in _drive)
                {
                    thisDrive.Add(new GameStreamPlayDTO(play));
                }

                return thisDrive;
            }
        }
        public int DriveFieldPositionStart { get; set; }
        public int DriveNumberOfPlays
        {
            get
            {
                return _drive.Where(p => p.Down != 0 && !p.Penalties.Any(pe => pe.NoPlay == true && pe.Enforced == true)).Count();
            }
        }
        public int DriveLength { get; set; }
        [JsonIgnore]
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
        [JsonIgnore]
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
