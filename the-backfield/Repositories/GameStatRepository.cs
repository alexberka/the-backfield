using Microsoft.EntityFrameworkCore;
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

    public async Task<GameStat?> GetSingleGameStatAsync(int gameStatId)
    {
        return await _dbContext.GameStats
            .AsNoTracking()
            .SingleOrDefaultAsync(gs => gs.Id == gameStatId);
    }
    public async Task<GameStat> CreateGameStatAsync(GameStat newGameStat)
    {
        _dbContext.GameStats.Add(newGameStat);
        await _dbContext.SaveChangesAsync();
        return newGameStat;
    }

    public Task<string?> DeleteGameStatAsync(int gameStatId, int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<GameStat?> UpdateGameStatAsync(GameStat updatedGameStat)
    {
        GameStat? gameStat = await _dbContext.GameStats.FindAsync(updatedGameStat.Id);
        if (gameStat == null)
        {
            return null;
        }

        gameStat.RushYards = updatedGameStat.RushYards;
        gameStat.RushAttempts = updatedGameStat.RushAttempts;
        gameStat.PassYards = updatedGameStat.PassYards;
        gameStat.PassAttempts = updatedGameStat.PassAttempts;
        gameStat.PassCompletions = updatedGameStat.PassCompletions;
        gameStat.ReceivingYards = updatedGameStat.ReceivingYards;
        gameStat.Receptions = updatedGameStat.Receptions;
        gameStat.Touchdowns = updatedGameStat.Touchdowns;
        gameStat.Tackles = updatedGameStat.Tackles;
        gameStat.InterceptionsThrown = updatedGameStat.InterceptionsThrown;
        gameStat.InterceptionsReceived = updatedGameStat.InterceptionsReceived;
        gameStat.FumblesCommitted = updatedGameStat.FumblesCommitted;
        gameStat.FumblesForced = updatedGameStat.FumblesForced;
        gameStat.FumblesRecovered = updatedGameStat.FumblesRecovered;

        await _dbContext.SaveChangesAsync();
        return gameStat;
    }
}