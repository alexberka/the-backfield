using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Repositories;
using TheBackfield.Utilities;

namespace TheBackfield.Services;

public class GameStatService : IGameStatService
{
    private readonly IGameStatRepository _gameStatRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IUserRepository _userRepository;

    public GameStatService(IGameStatRepository gameStatRepository, IGameRepository gameRepository, IPlayerRepository playerRepository, IUserRepository userRepository)
    {
        _gameStatRepository = gameStatRepository;
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _userRepository = userRepository;
    }

    public async Task<ResponseDTO<GameStat>> CreateGameStatAsync(GameStatSubmitDTO gameStatSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(gameStatSubmit.SessionKey);
        Game? game = await _gameRepository.GetSingleGameAsync(gameStatSubmit.GameId);
        ResponseDTO<Game> gameCheck = SessionKeyClient.VerifyAccess(gameStatSubmit.SessionKey, user, game);
        if (gameCheck.Error)
        {
            return gameCheck.ToType<GameStat>();
        }
        if (gameStatSubmit.TeamId != game.HomeTeamId && gameStatSubmit.TeamId != game.AwayTeamId)
        {
            return new ResponseDTO<GameStat> { ErrorMessage = "Team Id is not valid for this game" };
        }
        Player? player = await _playerRepository.GetSinglePlayerAsync(gameStatSubmit.PlayerId);
        ResponseDTO<Player> playerCheck = SessionKeyClient.VerifyAccess(gameStatSubmit.SessionKey, user, player);
        if (playerCheck.Error)
        {
            return playerCheck.ToType<GameStat>();
        }
        if (player.TeamId != gameStatSubmit.TeamId)
        {
            return new ResponseDTO<GameStat> { ErrorMessage = "Player Id is not valid for this team" };
        }

        return new ResponseDTO<GameStat> { Resource = await _gameStatRepository.CreateGameStatAsync(gameStatSubmit, user.Id) };
    }

    public async Task<ResponseDTO<GameStat>> DeleteGameStatAsync(int gameStatId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        GameStat? gameStat = await _gameStatRepository.GetSingleGameStatAsync(gameStatId);
        ResponseDTO<GameStat> gameStatCheck = SessionKeyClient.VerifyAccess(sessionKey, user, gameStat);
        if (gameStatCheck.Error)
        {
            return gameStatCheck;
        }

        return new ResponseDTO<GameStat> { ErrorMessage = await _gameStatRepository.DeleteGameStatAsync(gameStatId) };
    }

    public async Task<ResponseDTO<GameStat>> UpdateGameStatAsync(GameStatSubmitDTO gameStatSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(gameStatSubmit.SessionKey);
        GameStat? gameStat = await _gameStatRepository.GetSingleGameStatAsync(gameStatSubmit.Id);
        ResponseDTO<GameStat> gameStatCheck = SessionKeyClient.VerifyAccess(gameStatSubmit.SessionKey, user, gameStat);
        if (gameStatCheck.Error)
        {
            return gameStatCheck;
        }

        if (gameStatSubmit.GameId != 0 && gameStatSubmit.GameId != gameStat?.GameId)
        {
            return new ResponseDTO<GameStat> { ErrorMessage = "Game stat record cannot be reassigned to a different game. Instead, delete record and create a new one with the proper association." };
        }
        if (gameStatSubmit.TeamId != 0 && gameStatSubmit.TeamId != gameStat?.TeamId)
        {
            return new ResponseDTO<GameStat> { ErrorMessage = "Game stat record cannot be reassigned to a different team. Instead, delete record and create a new one with the proper association." };
        }
        if (gameStatSubmit.PlayerId != 0 && gameStatSubmit.PlayerId != gameStat?.PlayerId)
        {
            return new ResponseDTO<GameStat> { ErrorMessage = "Game stat record cannot be reassigned to a different player. Instead, delete record and create a new one with the proper association." };
        }

        return new ResponseDTO<GameStat> { Resource = await _gameStatRepository.UpdateGameStatAsync(gameStatSubmit) };
    }
}