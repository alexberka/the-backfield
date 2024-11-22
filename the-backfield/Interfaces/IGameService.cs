using TheBackfield.Models;
using TheBackfield.DTOs;

namespace TheBackfield.Interfaces;

public interface IGameService
{
    Task<GameResponseDTO> GetAllGamesAsync(string sessionKey);
    Task<GameResponseDTO> GetSingleGameAsync(int gameId, string sessionKey);
    Task<GameResponseDTO> CreateGameAsync(GameSubmitDTO gameSubmit);
    Task<GameResponseDTO> UpdateGameAsync(GameSubmitDTO gameSubmit);
    Task<string?> DeleteGameAsync(int gameId, int userId);
}