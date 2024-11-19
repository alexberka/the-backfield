using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Services;

public class GameStatService : IGameStatService
{
    private readonly IGameStatRepository _gameStatRepository;

    public GameStatService(IGameStatRepository gameStatRepository)
    {
        _gameStatRepository = gameStatRepository;
    }

    public async Task<GameStat> CreateGameStatAsync(GameStatSubmitDTO gameStatSubmit)
    {
        return await _gameStatRepository.CreateGameStatAsync(gameStatSubmit);
    }

    public async Task<string?> DeleteGameStatAsync(int gameStatId, int userId)
    {
        return await _gameStatRepository.DeleteGameStatAsync(gameStatId, userId);
    }

    public async Task<GameStat> UpdateGameStatAsync(GameStatSubmitDTO gameStatSubmit)
    {
        return await _gameStatRepository.UpdateGameStatAsync(gameStatSubmit);
    }
}