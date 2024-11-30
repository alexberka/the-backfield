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
    public async Task<GameStat> CreateGameStatAsync(GameStatSubmitDTO newGameStatSubmit, int userId)
    {
        GameStat newGameStat = new GameStat
        {
            GameId = newGameStatSubmit.GameId,
            PlayerId = newGameStatSubmit.PlayerId,
            TeamId = newGameStatSubmit.TeamId,
            RushYards = newGameStatSubmit.RushYards ?? 0,
            RushAttempts = newGameStatSubmit.RushAttempts ?? 0,
            PassYards = newGameStatSubmit.PassYards ?? 0,
            PassAttempts = newGameStatSubmit.PassAttempts ?? 0,
            PassCompletions = newGameStatSubmit.PassCompletions ?? 0,
            ReceivingYards = newGameStatSubmit.ReceivingYards ?? 0,
            Receptions = newGameStatSubmit.Receptions ?? 0,
            Touchdowns = newGameStatSubmit.Touchdowns ?? 0,
            Tackles = newGameStatSubmit.Tackles ?? 0,
            InterceptionsThrown = newGameStatSubmit.InterceptionsThrown ?? 0,
            InterceptionsReceived = newGameStatSubmit.InterceptionsReceived ?? 0,
            FumblesCommitted = newGameStatSubmit.FumblesCommitted ?? 0,
            FumblesForced = newGameStatSubmit.FumblesForced ?? 0,
            FumblesRecovered = newGameStatSubmit.FumblesRecovered ?? 0,
            UserId = userId
        };


        _dbContext.GameStats.Add(newGameStat);
        await _dbContext.SaveChangesAsync();
        return newGameStat;
    }

    public async Task<GameStat?> UpdateGameStatAsync(GameStatSubmitDTO updatedGameStatSubmit)
    {
        GameStat? gameStat = await _dbContext.GameStats.FindAsync(updatedGameStatSubmit.Id);
        if (gameStat == null)
        {
            return null;
        }

        gameStat.RushYards = updatedGameStatSubmit.RushYards ?? gameStat.RushYards;
        gameStat.RushAttempts = updatedGameStatSubmit.RushAttempts ?? gameStat.RushAttempts;
        gameStat.PassYards = updatedGameStatSubmit.PassYards ?? gameStat.PassYards;
        gameStat.PassAttempts = updatedGameStatSubmit.PassAttempts ?? gameStat.PassAttempts;
        gameStat.PassCompletions = updatedGameStatSubmit.PassCompletions ?? gameStat.PassCompletions;
        gameStat.ReceivingYards = updatedGameStatSubmit.ReceivingYards ?? gameStat.ReceivingYards;
        gameStat.Receptions = updatedGameStatSubmit.Receptions ?? gameStat.Receptions;
        gameStat.Touchdowns = updatedGameStatSubmit.Touchdowns ?? gameStat.Touchdowns;
        gameStat.Tackles = updatedGameStatSubmit.Tackles ?? gameStat.Tackles;
        gameStat.InterceptionsThrown = updatedGameStatSubmit.InterceptionsThrown ?? gameStat.InterceptionsThrown;
        gameStat.InterceptionsReceived = updatedGameStatSubmit.InterceptionsReceived ?? gameStat.InterceptionsReceived;
        gameStat.FumblesCommitted = updatedGameStatSubmit.FumblesCommitted ?? gameStat.FumblesCommitted;
        gameStat.FumblesForced = updatedGameStatSubmit.FumblesForced ?? gameStat.FumblesForced;
        gameStat.FumblesRecovered = updatedGameStatSubmit.FumblesRecovered ?? gameStat.FumblesRecovered;

        await _dbContext.SaveChangesAsync();
        return gameStat;
    }

    public async Task<string?> DeleteGameStatAsync(int gameStatId)
    {
        GameStat? gameStat = await _dbContext.GameStats.FindAsync(gameStatId);
        if (gameStat == null)
        {
            return "Invalid gameStat id";
        }

        _dbContext.GameStats.Remove(gameStat);
        await _dbContext.SaveChangesAsync();
        return null;
    }
}