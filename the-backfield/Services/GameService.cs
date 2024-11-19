using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;

    public GameService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<Game> CreateGameAsync(GameSubmitDTO gameSubmit)
    {
        return await _gameRepository.CreateGameAsync(gameSubmit);
    }

    public async Task<string?> DeleteGameAsync(int gameId, int userId)
    {
        return await _gameRepository.DeleteGameAsync(gameId, userId);
    }

    public async Task<List<Game>> GetAllGamesAsync(int userId)
    {
        return await _gameRepository.GetAllGamesAsync(userId);
    }

    public async Task<Game> GetSingleGameAsync(int gameId, int userId)
    {
        return await _gameRepository.GetSingleGameAsync(gameId, userId);
    }

    public async Task<Game> UpdateGameAsync(GameSubmitDTO gameSubmit)
    {
        return await _gameRepository.UpdateGameAsync(gameSubmit);
    }
}