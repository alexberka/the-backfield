using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class TackleRepository : ITackleRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public TackleRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }
    public async Task<Tackle?> CreateTackleAsync(int playId, int tacklerId)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .Include(p => p.Game)
            .SingleOrDefaultAsync(p => p.Id == playId);
        if (play == null)
        {
            return null;
        }

        Player? tackler = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == tacklerId);
        if (tackler == null || (tackler.TeamId != play.Game.HomeTeamId && tackler.TeamId != play.Game.AwayTeamId))
        {
            return null;
        }

        Tackle newTackle = new()
        {
            PlayId = playId,
            TacklerId = tacklerId
        };

        _dbContext.Tackles.Add(newTackle);
        await _dbContext.SaveChangesAsync();

        return newTackle;
    }
    public async Task<Tackle?> CreateTackleAsync(Tackle newTackle)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .Include(p => p.Game)
            .SingleOrDefaultAsync(p => p.Id == newTackle.PlayId);
        if (play == null || play.Game == null)
        {
            return null;
        }

        Player? tackler = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newTackle.TacklerId);
        if (tackler == null || !new int[] { play.Game.HomeTeamId, play.Game.AwayTeamId }.Contains(tackler.TeamId))
        {
            return null;
        }

        _dbContext.Tackles.Add(newTackle);
        await _dbContext.SaveChangesAsync();

        return newTackle;
    }

    public async Task<Tackle?> UpdateTackleAsync(Tackle tackleUpdate)
    {
        Tackle? tackle = await _dbContext.Tackles.SingleOrDefaultAsync(p => p.Id == tackleUpdate.Id);
        if (tackle == null)
        {
            return null;
        }

        if (tackle.TacklerId != null)
        {
            Player? tackler = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == tackleUpdate.TacklerId);
            if (tackler == null)
            {
                return null;
            }
        }

        tackle.TacklerId = tackleUpdate.TacklerId;
        tackle.TeamId = tackleUpdate.TeamId;

        await _dbContext.SaveChangesAsync();

        return tackle;
    }

    public async Task<string?> DeleteTackleAsync(int tackleId)
    {
        Tackle? tackle = await _dbContext.Tackles.FindAsync(tackleId);
        if (tackle == null)
        {
            return "Invalid tackle id";
        }

        _dbContext.Tackles.Remove(tackle);
        await _dbContext.SaveChangesAsync();
        return null;
    }
}