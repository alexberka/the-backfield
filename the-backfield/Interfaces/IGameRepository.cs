using TheBackfield.Models;
using TheBackfield.DTOs;

namespace TheBackfield.Interfaces;

public interface IGameRepository
{
    Task<List<Game>> GetAllGamesAsync(int userId);
    Task<Game?> GetSingleGameAsync(int gameId);
    Task<GameStreamDTO> GetGameStreamAsync(int gameId);
    Task<Game> CreateGameAsync(GameSubmitDTO newGameSubmit, int userId);
    Task<Game?> UpdateGameAsync(GameSubmitDTO updatedGameSubmit);
    Task<string?> DeleteGameAsync(int gameId);
}