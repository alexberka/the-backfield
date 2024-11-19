using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Repositories;

public class GameStatRepository : IGameStatRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public GameStatRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public Task<GameStat> CreateGameStatAsync(GameStatSubmitDTO gameStatSubmit)
    {
        throw new NotImplementedException();
    }

    public Task<string?> DeleteGameStatAsync(int gameStatId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<GameStat> UpdateGameStatAsync(GameStatSubmitDTO gameStatSubmit)
    {
        throw new NotImplementedException();
    }
}