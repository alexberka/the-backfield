using TheBackfield.Models;
using TheBackfield.DTOs;

namespace TheBackfield.Interfaces;

public interface IGameRepository
{
    Task<List<Game>> GetAllGamesAsync(int userId);
    Task<Game?> GetSingleGameAsync(int gameId);
    Task<Game> CreateGameAsync(Game newGame);
    Task<Game?> UpdateGameAsync(Game updatedGame);
    Task<string?> DeleteGameAsync(int gameId, int userId);
}