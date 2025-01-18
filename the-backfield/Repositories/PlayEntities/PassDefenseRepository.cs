using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class PassDefenseRepository : IPassDefenseRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public PassDefenseRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }
    public async Task<PassDefense?> CreatePassDefenseAsync(int playId, int defenderId)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .Include(p => p.Game)
            .SingleOrDefaultAsync(p => p.Id == playId);
        if (play == null)
        {
            return null;
        }

        int defensiveTeamId = play.TeamId == play.Game.HomeTeamId ? play.Game.AwayTeamId : play.Game.HomeTeamId;

        Player? defender = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == defenderId);
        if (defender == null || defender.TeamId != defensiveTeamId)
        {
            return null;
        }

        PassDefense newPassDefense = new()
        {
            PlayId = playId,
            DefenderId = defenderId
        };

        _dbContext.PassDefenses.Add(newPassDefense);
        await _dbContext.SaveChangesAsync();

        return newPassDefense;
    }
    public async Task<PassDefense?> CreatePassDefenseAsync(PassDefense newPassDefense)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .Include(p => p.Game)
            .SingleOrDefaultAsync(p => p.Id == newPassDefense.PlayId);
        if (play == null || play.Game == null)
        {
            return null;
        }

        int defensiveTeamId = play.TeamId == play.Game.HomeTeamId ? play.Game.AwayTeamId : play.Game.HomeTeamId;

        Player? defender = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newPassDefense.DefenderId);
        if (defender == null || defender.TeamId != defensiveTeamId)
        {
            return null;
        }

        _dbContext.PassDefenses.Add(newPassDefense);
        await _dbContext.SaveChangesAsync();

        return newPassDefense;
    }

    public Task<bool> DeletePassDefenseAsync(int passDefenseId)
    {
        throw new NotImplementedException();
    }
}