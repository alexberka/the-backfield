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

    public GameService(
        IGameRepository gameRepository,
        IPlayRepository playRepository,
        ITeamRepository teamRepository,
        IUserRepository userRepository
        )
    {
        _gameRepository = gameRepository;
        _playRepository = playRepository;
        _teamRepository = teamRepository;
        _userRepository = userRepository;
    }

    public async Task<GameResponseDTO> CreateGameAsync(GameSubmitDTO gameSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(gameSubmit.SessionKey);
        if (user == null)
        {
            return new GameResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }

        if (gameSubmit.HomeTeamId == gameSubmit.AwayTeamId)
        {
            return new GameResponseDTO { ErrorMessage = "Home team and away team cannot be identical" };
        }

        Team? homeTeam = await _teamRepository.GetSingleTeamAsync(gameSubmit.HomeTeamId);
        TeamResponseDTO teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey, user, homeTeam);
        if (teamCheck.ErrorMessage != null)
        {
            return teamCheck.CastToGameResponseDTO();
        }

        Team? awayTeam = await _teamRepository.GetSingleTeamAsync(gameSubmit.AwayTeamId);
        teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey, user, awayTeam);
        if (teamCheck.ErrorMessage != null)
        {
            return teamCheck.CastToGameResponseDTO();
        }

        return new GameResponseDTO { Game = await _gameRepository.CreateGameAsync(gameSubmit, user.Id) };
    }

    public async Task<GameResponseDTO> DeleteGameAsync(int gameId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Game? game = await _gameRepository.GetSingleGameAsync(gameId);
        GameResponseDTO gameCheck = SessionKeyClient.VerifyAccess(sessionKey, user, game);
        if (gameCheck.Error)
        {
            return gameCheck;
        }

        return new GameResponseDTO { ErrorMessage = await _gameRepository.DeleteGameAsync(gameId) };
    }

    public async Task<GameResponseDTO> GetAllGamesAsync(string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        if (user == null)
        {
            return new GameResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }
        return new GameResponseDTO { Games = await _gameRepository.GetAllGamesAsync(user.Id) };
    }

    public async Task<GameResponseDTO> GetSingleGameAsync(int gameId, string sessionKey)
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
        int? fieldPositionEnd = null;
        int? nextTeamId = null;

        int currentPlayId = game.Plays.SingleOrDefault(p => !game.Plays.Any(gp => gp.PrevPlayId == p.Id))?.Id ?? 0;
        Play? currentPlay = await _playRepository.GetSinglePlayAsync(currentPlayId);
        var (homeTeamScore, awayTeamScore) = StatClient.ParseScore(game);

        if (currentPlay != null)
        {
            (down, toGain, fieldPositionEnd, nextTeamId) = StatClient.ParseFieldPosition(currentPlay, game.HomeTeamId, game.AwayTeamId);
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
            FieldPositionStart = fieldPositionEnd,
            Down = down,
            ToGain = toGain,
            ClockStart = clockStart,
            GamePeriod = gamePeriod
        };

        GameStreamDTO gameStream = new(game, nextPlay);

        return gameStream;
    }

    public async Task<GameResponseDTO> UpdateGameAsync(GameSubmitDTO gameSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(gameSubmit.SessionKey);
        Game? game = await _gameRepository.GetSingleGameAsync(gameSubmit.Id);
        GameResponseDTO gameCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey, user, game);
        if (gameCheck.Error)
        {
            return gameCheck;
        }

        if ((gameSubmit.HomeTeamId != 0 ? gameSubmit.HomeTeamId : game.HomeTeamId) == (gameSubmit.AwayTeamId != 0 ? gameSubmit.AwayTeamId : game.AwayTeamId))
        {
            return new GameResponseDTO { ErrorMessage = "Home team and away team cannot be identical" };
        }

        if (gameSubmit.HomeTeamId != 0 && gameSubmit.HomeTeamId != game.HomeTeamId)
        {
            Team? homeTeam = await _teamRepository.GetSingleTeamAsync(gameSubmit.HomeTeamId);
            TeamResponseDTO teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey, user, homeTeam);
            if (teamCheck.Error)
            {
                return teamCheck.CastToGameResponseDTO();
            }
        }

        if (gameSubmit.AwayTeamId != 0 && gameSubmit.AwayTeamId != game.AwayTeamId)
        {
            Team? awayTeam = await _teamRepository.GetSingleTeamAsync(gameSubmit.AwayTeamId);
            TeamResponseDTO teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey, user, awayTeam);
            if (teamCheck.Error)
            {
                return teamCheck.CastToGameResponseDTO();
            }
        }

        return new GameResponseDTO { Game = await _gameRepository.UpdateGameAsync(gameSubmit) };
    }
}