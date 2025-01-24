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

    public async Task<PassDefense?> UpdatePassDefenseAsync(PassDefense passDefenseUpdate)
    {
        PassDefense? passDefense = await _dbContext.PassDefenses.SingleOrDefaultAsync(p => p.Id == passDefenseUpdate.Id);
        if (passDefense == null)
        {
            return null;
        }

        if (passDefenseUpdate.DefenderId != null)
        {
            Player? defender = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == passDefenseUpdate.DefenderId);
            if (defender == null)
            {
                return null;
            }
        }

        passDefense.DefenderId = passDefenseUpdate.DefenderId;
        passDefense.TeamId = passDefenseUpdate.TeamId;

        await _dbContext.SaveChangesAsync();

        return passDefense;
    }

    public async Task<string?> DeletePassDefenseAsync(int passDefenseId)
    {
        PassDefense? passDefense = await _dbContext.PassDefenses.FindAsync(passDefenseId);
        if (passDefense == null)
        {
            return "Invalid pass defense id";
        }

        _dbContext.PassDefenses.Remove(passDefense);
        await _dbContext.SaveChangesAsync();
        return null;
    }
}