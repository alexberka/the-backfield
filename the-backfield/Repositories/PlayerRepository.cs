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

    public Task<Player> CreatePlayerAsync(PlayerSubmitDTO playerSubmit)
    {
        throw new NotImplementedException();
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