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

    public Task<bool> DeleteTackleAsync(int tackleId)
    {
        throw new NotImplementedException();
    }
}