using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IGameStatRepository
{
    Task<GameStat?> GetSingleGameStatAsync(int gameStatId);
    Task<GameStat> CreateGameStatAsync(GameStatSubmitDTO newGameStatSubmit, int userId);
    Task<GameStat?> UpdateGameStatAsync(GameStatSubmitDTO updatedGameStatSubmit);
    Task<string?> DeleteGameStatAsync(int gameStatId);
}