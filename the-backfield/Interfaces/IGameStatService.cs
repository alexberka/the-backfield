using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IGameStatService
{
    Task<GameStatResponseDTO> CreateGameStatAsync(GameStatSubmitDTO gameStatSubmit);
    Task<GameStatResponseDTO> UpdateGameStatAsync(GameStatSubmitDTO gameStatSubmit);
    Task<GameStatResponseDTO> DeleteGameStatAsync(int gameStatId, string sessionKey);
}