using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IGameStatRepository
{
    Task<GameStat?> GetSingleGameStatAsync(int gameStatId);
    Task<GameStat> CreateGameStatAsync(GameStat newGameStat);
    Task<GameStat?> UpdateGameStatAsync(GameStat updatedGameStat);
    Task<string?> DeleteGameStatAsync(int gameStatId, int userId);
}