using TheBackfield.DTOs.GameStream;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Utilities;
using System.Xml.Linq;

namespace TheBackfield.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IPlayRepository _playRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPlayService _playService;

    public GameService(
        IGameRepository gameRepository,
        IPlayRepository playRepository,
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        IPlayService playService
        )
    {
        _gameRepository = gameRepository;
        _playRepository = playRepository;
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _playService = playService;
    }

    public async Task<ResponseDTO<Game>> CreateGameAsync(GameSubmitDTO gameSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(gameSubmit.SessionKey ?? "");
        if (user == null)
        {
            return new ResponseDTO<Game> { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }

        if (gameSubmit.HomeTeamId == gameSubmit.AwayTeamId)
        {
            return new ResponseDTO<Game> { ErrorMessage = "Home team and away team cannot be identical" };
        }

        Team? homeTeam = await _teamRepository.GetSingleTeamAsync(gameSubmit.HomeTeamId);
        ResponseDTO<Team> teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey ?? "", user, homeTeam);
        if (teamCheck.ErrorMessage != null)
        {
            return teamCheck.ToType<Game>();
        }

        Team? awayTeam = await _teamRepository.GetSingleTeamAsync(gameSubmit.AwayTeamId);
        teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey ?? "", user, awayTeam);
        if (teamCheck.ErrorMessage != null)
        {
            return teamCheck.ToType<Game>();
        }

        return new ResponseDTO<Game> { Resource = await _gameRepository.CreateGameAsync(gameSubmit, user.Id) };
    }

    public async Task<ResponseDTO<Game>> DeleteGameAsync(int gameId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Game? game = await _gameRepository.GetSingleGameAsync(gameId);
        ResponseDTO<Game> gameCheck = SessionKeyClient.VerifyAccess(sessionKey, user, game);
        if (gameCheck.Error)
        {
            return gameCheck;
        }

        return new ResponseDTO<Game> { ErrorMessage = await _gameRepository.DeleteGameAsync(gameId) };
    }

    public async Task<ResponseDTO<List<Game>>> GetAllGamesAsync(string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        if (user == null)
        {
            return new ResponseDTO<List<Game>> { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }
        return new ResponseDTO<List<Game>> { Resource = await _gameRepository.GetAllGamesAsync(user.Id) };
    }

    public async Task<ResponseDTO<Game>> GetSingleGameAsync(int gameId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Game? game = await _gameRepository.GetSingleGameAsync(gameId);
        return SessionKeyClient.VerifyAccess(sessionKey, user, game);
    }

    public async Task<GameStreamDTO?> GetGameStreamAsync(int gameId)
    {
        Game? game = await _gameRepository.GetSingleGameAllStatsAsync(gameId);
        if (game == null)
        {
            return null;
        }

        int down = 0;
        int? toGain = null;
        int? fieldPositionStart = null;
        int? nextTeamId = null;
        PlayAsSegmentsDTO? lastPlay = null;

        int currentPlayId = game.Plays.SingleOrDefault(p => !game.Plays.Any(gp => gp.PrevPlayId == p.Id))?.Id ?? 0;
        Play? currentPlay = await _playRepository.GetSinglePlayAsync(currentPlayId);
        var (homeTeamScore, awayTeamScore) = StatClient.ParseScore(game);

        if (currentPlay != null)
        {
            (down, toGain, fieldPositionStart, nextTeamId) = StatClient.ParseNextFieldPosition(currentPlay, game.HomeTeamId, game.AwayTeamId);
            lastPlay = new PlayAsSegmentsDTO(currentPlay, await _playService.GetPlaySegmentsAsync(currentPlayId));
        }

        int? clockStart = null;
        int? gamePeriod = null;
        int playCheckId = currentPlay?.Id ?? 0;
        do
        {
            Play? checkPlay = game.Plays.SingleOrDefault(p => p.Id == playCheckId);
            if (checkPlay == null)
            {
                clockStart = game.PeriodLength;
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
                    clockStart = game.PeriodLength;
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
            clockStart = game.PeriodLength;
        }

        PlaySubmitDTO nextPlay = new()
        {
            PrevPlayId = currentPlay?.Id ?? -1,
            GameId = game.Id,
            TeamId = nextTeamId ?? 0,
            FieldPositionStart = fieldPositionStart,
            Down = down,
            ToGain = toGain,
            ClockStart = clockStart,
            GamePeriod = gamePeriod
        };

        GameStreamDTO gameStream = new(game, nextPlay);

        gameStream.HomeTeamScore = homeTeamScore;
        gameStream.AwayTeamScore = awayTeamScore;
        gameStream.LastPlay = lastPlay;

        // The drive always has at least one play in it (that play may be an empty play if at start of game or currentPlay is otherwise null)
        List<Play> drive = [currentPlay ?? new()];

        bool driveFound = currentPlay?.TeamId != nextTeamId;

        // Collect all plays from current drive, including kickoff to start drive (does not count as a play)
        while (drive[0].Kickoff == null && drive[0].PrevPlayId > 0 && !driveFound)
        {
            Play? previousPlay = game.Plays.SingleOrDefault((p) => p.Id == drive[0].PrevPlayId);
            if (previousPlay == null)
            {
                driveFound = true;
            }
            else if (previousPlay.TeamId != nextTeamId && previousPlay.Kickoff == null)
            {
                driveFound = true;
            }
            else
            {
                drive.Insert(0, previousPlay);
            }
        }

        // Remove kickoffs, turnovers, empty plays, or plays that were nullified by penalties
        List<Play> countedPlays = drive
            .Where((p) => p.Kickoff == null
                && p.TeamId == nextPlay.TeamId
                && p.PrevPlayId != null
                && !p.Penalties.Any((pp) => pp.Enforced == true && pp.NoPlay == true))
            .ToList();

        gameStream.DrivePlays = countedPlays.Count;
        gameStream.DrivePositionStart = nextPlay.FieldPositionStart;
        gameStream.DriveYards = 0;
        gameStream.DriveTime = 0;

        if (gameStream.DrivePlays > 0)
        {
            int driveTimeStart = 0;
            int driveTimeEnd = (game.GamePeriods - (nextPlay.GamePeriod ?? 0)) * game.PeriodLength + (nextPlay.ClockStart ?? 0);

            gameStream.DrivePositionStart = countedPlays[0].FieldPositionStart;

            driveTimeStart = (game.GamePeriods - (countedPlays[0].GamePeriod ?? 0)) * game.PeriodLength + (countedPlays[0].ClockStart ?? 0);

            gameStream.DriveTime = driveTimeStart - driveTimeEnd;
            // If last play was a made field goal, or a turnover, count drive yards up to start of last play
            if ((countedPlays[countedPlays.Count - 1].FieldGoal?.Good ?? false)
                || (nextTeamId != countedPlays[countedPlays.Count -1].TeamId))
            {
                gameStream.DriveYards = ((gameStream.DrivePositionStart - countedPlays[countedPlays.Count - 1].FieldPositionStart) * (currentPlay?.TeamId == game?.HomeTeamId ? -1 : 1)) ?? 0;
            }
            // else count to end
            else
            {
                gameStream.DriveYards = ((gameStream.DrivePositionStart - countedPlays[countedPlays.Count - 1].FieldPositionEnd) * (currentPlay?.TeamId == game?.HomeTeamId ? -1 : 1)) ?? 0;
            }
        }

        return gameStream;
    }

    public async Task<ResponseDTO<Game>> UpdateGameAsync(GameSubmitDTO gameSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(gameSubmit.SessionKey ?? "");
        Game? game = await _gameRepository.GetSingleGameAsync(gameSubmit.Id);
        ResponseDTO<Game> gameCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey ?? "", user, game);
        if (gameCheck.Error)
        {
            return gameCheck;
        }

        if ((gameSubmit.HomeTeamId != 0 ? gameSubmit.HomeTeamId : game.HomeTeamId) == (gameSubmit.AwayTeamId != 0 ? gameSubmit.AwayTeamId : game.AwayTeamId))
        {
            return new ResponseDTO<Game> { ErrorMessage = "Home team and away team cannot be identical" };
        }

        if (gameSubmit.HomeTeamId != 0 && gameSubmit.HomeTeamId != game.HomeTeamId)
        {
            Team? homeTeam = await _teamRepository.GetSingleTeamAsync(gameSubmit.HomeTeamId);
            ResponseDTO<Team> teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey, user, homeTeam);
            if (teamCheck.Error)
            {
                return teamCheck.ToType<Game>();
            }
        }

        if (gameSubmit.AwayTeamId != 0 && gameSubmit.AwayTeamId != game.AwayTeamId)
        {
            Team? awayTeam = await _teamRepository.GetSingleTeamAsync(gameSubmit.AwayTeamId);
            ResponseDTO<Team> teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey ?? "", user, awayTeam);
            if (teamCheck.Error)
            {
                return teamCheck.ToType<Game>();
            }
        }

        return new ResponseDTO<Game> { Resource = await _gameRepository.UpdateGameAsync(gameSubmit) };
    }
}