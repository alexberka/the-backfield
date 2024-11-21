using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public PlayerRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Player> CreatePlayerAsync(Player newPlayer)
    {
        _dbContext.Players.Add(newPlayer);
        await _dbContext.SaveChangesAsync();
        return newPlayer;
    }

    public Task<string?> DeletePlayerAsync(int playerId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Player>> GetPlayersAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<Player> GetSinglePlayerAsync(int playerId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<Player> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit)
    {
        throw new NotImplementedException();
    }
}