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
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;

    public GameStatService(IGameStatRepository gameStatRepository, IGameRepository gameRepository, IPlayerRepository playerRepository, ITeamRepository teamRepository, IUserRepository userRepository)
    {
        _gameStatRepository = gameStatRepository;
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _teamRepository = teamRepository;
        _userRepository = userRepository;
    }

    public async Task<GameStatResponseDTO> CreateGameStatAsync(GameStatSubmitDTO gameStatSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(gameStatSubmit.SessionKey);
        Game? game = await _gameRepository.GetSingleGameAsync(gameStatSubmit.GameId);
        GameResponseDTO gameCheck = SessionKeyClient.VerifyAccess(gameStatSubmit.SessionKey, user, game);
        if (gameCheck.Error)
        {
            return gameCheck.CastTo<GameStatResponseDTO>();
        }
        if (gameStatSubmit.TeamId != game.HomeTeamId && gameStatSubmit.TeamId != game.AwayTeamId)
        {
            return new GameStatResponseDTO { ErrorMessage = "Team Id is not valid for this game" };
        }
        Player? player = await _playerRepository.GetSinglePlayerAsync(gameStatSubmit.PlayerId);
        PlayerResponseDTO playerCheck = SessionKeyClient.VerifyAccess(gameStatSubmit.SessionKey, user, player);
        if (playerCheck.Error)
        {
            return playerCheck.CastTo<GameStatResponseDTO>();
        }
        if (player.TeamId != gameStatSubmit.TeamId)
        {
            return new GameStatResponseDTO { ErrorMessage = "Player Id is not valid for this team" };
        }

        return new GameStatResponseDTO { GameStat = await _gameStatRepository.CreateGameStatAsync(gameStatSubmit.MapToGameStat(user.Id)) };
    }

    public async Task<GameStatResponseDTO> DeleteGameStatAsync(int gameStatId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        GameStat? gameStat = await _gameStatRepository.GetSingleGameStatAsync(gameStatId);
        GameStatResponseDTO gameStatCheck = SessionKeyClient.VerifyAccess(sessionKey, user, gameStat);
        if (gameStatCheck.Error)
        {
            return gameStatCheck;
        }

        return new GameStatResponseDTO { ErrorMessage = await _gameStatRepository.DeleteGameStatAsync(gameStatId) };
    }

    public async Task<GameStatResponseDTO> UpdateGameStatAsync(GameStatSubmitDTO gameStatSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(gameStatSubmit.SessionKey);
        GameStat? gameStat = await _gameStatRepository.GetSingleGameStatAsync(gameStatSubmit.Id);
        GameStatResponseDTO gameStatCheck = SessionKeyClient.VerifyAccess(gameStatSubmit.SessionKey, user, gameStat);
        if (gameStatCheck.Error)
        {
            return gameStatCheck;
        }

        if (gameStatSubmit.GameId != 0 && gameStatSubmit.GameId != gameStat.GameId)
        {
            return new GameStatResponseDTO { ErrorMessage = "Game stat record cannot be reassigned to a different game. Instead, delete record and create a new one with the proper association." };
        }
        if (gameStatSubmit.TeamId != 0 && gameStatSubmit.TeamId != gameStat.TeamId)
        {
            return new GameStatResponseDTO { ErrorMessage = "Game stat record cannot be reassigned to a different team. Instead, delete record and create a new one with the proper association." };
        }
        if (gameStatSubmit.PlayerId != 0 && gameStatSubmit.PlayerId != gameStat.PlayerId)
        {
            return new GameStatResponseDTO { ErrorMessage = "Game stat record cannot be reassigned to a different player. Instead, delete record and create a new one with the proper association." };
        }

        GameStat updatedGameStat = gameStatSubmit.MapToGameStat(user.Id, gameStat);

        return new GameStatResponseDTO { GameStat = await _gameStatRepository.UpdateGameStatAsync(updatedGameStat) };
    }
}