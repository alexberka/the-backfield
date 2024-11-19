using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IGameStatService
{
    Task<GameStat> CreateGameStatAsync(GameStatSubmitDTO gameStatSubmit);
    Task<GameStat> UpdateGameStatAsync(GameStatSubmitDTO gameStatSubmit);
    Task<string?> DeleteGameStatAsync(int gameStatId, int userId);
}