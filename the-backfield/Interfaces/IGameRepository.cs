using TheBackfield.Models;
using TheBackfield.DTOs;

namespace TheBackfield.Interfaces;

public interface IGameRepository
{
    Task<List<Game>> GetAllGamesAsync(int userId);
    Task<Game> GetSingleGameAsync(int gameId, int userId);
    Task<Game> CreateGameAsync(GameSubmitDTO gameSubmit);
    Task<Game> UpdateGameAsync(GameSubmitDTO gameSubmit);
    Task<string?> DeleteGameAsync(int gameId, int userId);
}