using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Utilities;

namespace TheBackfield.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserRepository _userRepository;

    public GameService(IGameRepository gameRepository, IUserRepository userRepository)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
    }

    public async Task<GameResponseDTO> CreateGameAsync(GameSubmitDTO gameSubmit)
    {
        return new GameResponseDTO { Game = await _gameRepository.CreateGameAsync(gameSubmit) };
    }

    public async Task<string?> DeleteGameAsync(int gameId, int userId)
    {
        return await _gameRepository.DeleteGameAsync(gameId, userId);
    }

    public async Task<GameResponseDTO> GetAllGamesAsync(string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        if (user == null)
        {
            return new GameResponseDTO { Unauthorized = true, ErrorMessage = "Invalid user id" };
        }
        return new GameResponseDTO { Games = await _gameRepository.GetAllGamesAsync(user.Id) };
    }

    public async Task<GameResponseDTO> GetSingleGameAsync(int gameId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Game? game = await _gameRepository.GetSingleGameAsync(gameId);
        return SessionKeyClient.VerifyAccess(sessionKey, user, game);
    }

    public async Task<GameResponseDTO> UpdateGameAsync(GameSubmitDTO gameSubmit)
    {
        return new GameResponseDTO { Game = await _gameRepository.UpdateGameAsync(gameSubmit) };
    }
}