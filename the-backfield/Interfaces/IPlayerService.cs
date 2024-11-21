using TheBackfield.DTOs;

namespace TheBackfield.Interfaces;

public interface IPlayerService
{
    Task<PlayerResponseDTO> GetPlayersAsync(string sessionKey);
    Task<PlayerResponseDTO> GetSinglePlayerAsync(int playerId, string sessionKey);
    Task<PlayerResponseDTO> CreatePlayerAsync(PlayerSubmitDTO playerSubmit);
    Task<PlayerResponseDTO> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit);
    Task<string?> DeletePlayerAsync(int playerId, int userId);
}