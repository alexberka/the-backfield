using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IGameStatRepository
{
    Task<GameStat> CreateGameStatAsync(GameStatSubmitDTO gameStatSubmit);
    Task<GameStat> UpdateGameStatAsync(GameStatSubmitDTO gameStatSubmit);
    Task<string?> DeleteGameStatAsync(int gameStatId, int userId);
}