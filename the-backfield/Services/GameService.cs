using TheBackfield.DTOs.GameStream;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Utilities;

namespace TheBackfield.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;

    public GameService(
        IGameRepository gameRepository,
        ITeamRepository teamRepository,
        IUserRepository userRepository
        )
    {
        _gameRepository = gameRepository;
        _teamRepository = teamRepository;
        _userRepository = userRepository;
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