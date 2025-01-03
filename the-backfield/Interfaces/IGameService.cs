using TheBackfield.Models;
using TheBackfield.DTOs;
using TheBackfield.DTOs.GameStream;

namespace TheBackfield.Interfaces;

public interface IGameService
{
    Task<GameResponseDTO> GetAllGamesAsync(string sessionKey);
    Task<GameResponseDTO> GetSingleGameAsync(int gameId, string sessionKey);
    Task<GameStreamDTO?> GetGameStreamAsync(int gameId);
    Task<GameResponseDTO> CreateGameAsync(GameSubmitDTO gameSubmit);
    Task<GameResponseDTO> UpdateGameAsync(GameSubmitDTO gameSubmit);
    Task<GameResponseDTO> DeleteGameAsync(int gameId, string sessionKey);
}