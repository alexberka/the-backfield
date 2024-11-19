using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Repositories;

public class GameRepository : IGameRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public GameRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public Task<List<Game>> GetAllGamesAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<Game> GetSingleGameAsync(int gameId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<Game> CreateGameAsync(GameSubmitDTO gameSubmit)
    {
        throw new NotImplementedException();
    }

    public Task<Game> UpdateGameAsync(GameSubmitDTO gameSubmit)
    {
        throw new NotImplementedException();
    }

    public Task<string?> DeleteGameAsync(int gameId, int userId)
    {
        throw new NotImplementedException();
    }
}