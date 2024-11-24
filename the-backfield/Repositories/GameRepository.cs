using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Game>> GetAllGamesAsync(int userId)
    {
        return await _dbContext.Games
            .AsNoTracking()
            .Where(g => g.UserId == userId)
            .ToListAsync();
    }

    public async Task<Game?> GetSingleGameAsync(int gameId)
    {
        return await _dbContext.Games
            .AsNoTracking()
            .Include(g => g.HomeTeam)
            .Include(g => g.AwayTeam)
            .SingleOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task<Game> CreateGameAsync(Game newGame)
    {
        _dbContext.Games.Add(newGame);
        await _dbContext.SaveChangesAsync();
        return newGame;
    }

    public async Task<Game?> UpdateGameAsync(Game updatedGame)
    {
        Game? game = await _dbContext.Games.FindAsync(updatedGame.Id);
        if (game == null)
        {
            return null;
        }

        game.HomeTeamId = updatedGame.HomeTeamId;
        game.HomeTeamScore = updatedGame.HomeTeamScore;
        game.AwayTeamId = updatedGame.AwayTeamId;
        game.AwayTeamScore = updatedGame.AwayTeamScore;
        game.GameStart = updatedGame.GameStart;
        game.GamePeriods = updatedGame.GamePeriods;
        game.PeriodLength = updatedGame.PeriodLength;

        await _dbContext.SaveChangesAsync();
        return game;
    }

    public Task<string?> DeleteGameAsync(int gameId, int userId)
    {
        throw new NotImplementedException();
    }
}