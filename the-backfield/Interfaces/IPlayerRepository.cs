using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IPlayerRepository
{
    Task<List<Player>> GetPlayersAsync(int userId);
    Task<Player> GetSinglePlayerAsync(int playerId, int userId);
    Task<Player> CreatePlayerAsync(Player newPlayer);
    Task<Player> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit);
    Task<string?> DeletePlayerAsync(int playerId, int userId);
}