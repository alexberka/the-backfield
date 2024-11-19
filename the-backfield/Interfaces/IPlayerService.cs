using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IPlayerService
{
    Task<List<Player>> GetPlayersAsync(int userId);
    Task<Player> GetSinglePlayerAsync(int playerId, int userId);
    Task<Player> CreatePlayerAsync(PlayerSubmitDTO playerSubmit);
    Task<Player> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit);
    Task<string?> DeletePlayerAsync(int playerId, int userId);
}