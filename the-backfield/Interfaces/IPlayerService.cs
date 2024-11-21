using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IPlayerService
{
    Task<List<Player>> GetPlayersAsync(int userId);
    Task<Player> GetSinglePlayerAsync(int playerId, int userId);
    Task<PlayerResponseDTO> CreatePlayerAsync(PlayerSubmitDTO playerSubmit);
    Task<PlayerResponseDTO> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit);
    Task<string?> DeletePlayerAsync(int playerId, int userId);
}