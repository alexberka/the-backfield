using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IPlayerRepository
{
    Task<List<Player>> GetPlayersAsync(int userId);
    Task<Player?> GetSinglePlayerAsync(int playerId);
    Task<Player> CreatePlayerAsync(PlayerSubmitDTO playerSubmit, int userId);
    Task<Player?> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit);
    Task<string?> DeletePlayerAsync(int playerId);
    Task<Player?> SetPlayerPositionsAsync(int playerId, List<int> positionIds);
    Task<Player?> AddPlayerPositionsAsync(int playerId, List<int> positionIds);
    Task<Player?> RemovePlayerPositionsAsync(int playerId, List<int> positionIds);

}