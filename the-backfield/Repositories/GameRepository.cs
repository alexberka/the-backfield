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
            .Include(g => g.GameStats)
            .Include(g => g.Plays)
                .ThenInclude(p => p.Pass)
                    .ThenInclude(p => p.Passer)
            .Include(g => g.Plays)
                .ThenInclude(p => p.Rush)
            .Include(g => g.Plays)
                .ThenInclude(p => p.Tacklers)
                    .ThenInclude(t => t.Tackler)
            .Include(g => g.Plays)
                .ThenInclude(p => p.PassDefenders)
                    .ThenInclude(pd => pd.Defender)
            .Include(g => g.Plays)
                .ThenInclude(p => p.Kickoff)
            .Include(g => g.Plays)
                .ThenInclude(p => p.Punt)
            .Include(g => g.Plays)
                .ThenInclude(p => p.FieldGoal)
            .Include(g => g.Plays)
                .ThenInclude(g => g.Touchdown)
            .Include(g => g.Plays)
                .ThenInclude(g => g.ExtraPoint)
            .Include(g => g.Plays)
                .ThenInclude(p => p.Conversion)
            .Include(g => g.Plays)
                .ThenInclude(g => g.Safety)
            .Include(g => g.Plays)
                .ThenInclude(g => g.Fumbles)
            .Include(g => g.Plays)
                .ThenInclude(p => p.Interception)
            .Include(g => g.Plays)
                .ThenInclude(p => p.KickBlock)
            .Include(g => g.Plays)
                .ThenInclude(p => p.Laterals)
            .Include(g => g.Plays)
                .ThenInclude(p => p.Penalties)
            .SingleOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task<Game> CreateGameAsync(GameSubmitDTO newGameSubmit, int userId)
    {
        Game newGame = new Game
        {
            HomeTeamId = newGameSubmit.HomeTeamId,
            HomeTeamScore = newGameSubmit.HomeTeamScore ?? 0,
            AwayTeamId = newGameSubmit.AwayTeamId,
            AwayTeamScore = newGameSubmit.AwayTeamScore ?? 0,
            GameStart = newGameSubmit.GameStart ?? DateTime.MinValue,
            GamePeriods = newGameSubmit.GamePeriods ?? 1,
            PeriodLength = newGameSubmit.PeriodLength ?? 0,
            UserId = userId,
        };

        _dbContext.Games.Add(newGame);
        await _dbContext.SaveChangesAsync();
        return newGame;
    }

    public async Task<Game?> UpdateGameAsync(GameSubmitDTO updatedGameSubmit)
    {
        Game? game = await _dbContext.Games.FindAsync(updatedGameSubmit.Id);
        if (game == null)
        {
            return null;
        }

        game.HomeTeamId = updatedGameSubmit.HomeTeamId != 0 ? updatedGameSubmit.HomeTeamId : game.HomeTeamId;
        game.HomeTeamScore = updatedGameSubmit.HomeTeamScore ?? game.HomeTeamScore;
        game.AwayTeamId = updatedGameSubmit.AwayTeamId != 0 ? updatedGameSubmit.AwayTeamId : game.AwayTeamId;
        game.AwayTeamScore = updatedGameSubmit.AwayTeamScore ?? game.AwayTeamScore;
        game.GameStart = updatedGameSubmit.GameStart ?? game.GameStart;
        game.GamePeriods = updatedGameSubmit.GamePeriods ?? game.GamePeriods;
        game.PeriodLength = updatedGameSubmit.PeriodLength ?? game.PeriodLength;

        await _dbContext.SaveChangesAsync();
        return game;
    }

    public async Task<string?> DeleteGameAsync(int gameId)
    {
        Game? game = await _dbContext.Games.FindAsync(gameId);
        if (game == null)
        {
            return "Invalid game id";
        }

        _dbContext.Games.Remove(game);
        await _dbContext.SaveChangesAsync();
        return null;
    }
}