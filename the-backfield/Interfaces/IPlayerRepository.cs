using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IPlayerRepository
{
    Task<List<Player>> GetPlayersAsync(int userId);
    Task<Player?> GetSinglePlayerAsync(int playerId);
    Task<Player> CreatePlayerAsync(Player newPlayer);
    Task<Player?> UpdatePlayerAsync(Player updatedPlayer);
    Task<string?> DeletePlayerAsync(int playerId, int userId);
    Task<Player?> SetPlayerPositionsAsync(int playerId, List<int> positionIds);
    Task<Player?> AddPlayerPositionsAsync(int playerId, List<int> positionIds);
    Task<Player?> RemovePlayerPositionsAsync(int playerId, List<int> positionIds);

}