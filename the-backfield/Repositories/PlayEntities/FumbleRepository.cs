using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class FumbleRepository : IFumbleRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public FumbleRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }
    public async Task<Fumble?> CreateFumbleAsync(FumbleSubmitDTO fumbleSubmit)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .Include(p => p.Game)
            .SingleOrDefaultAsync(p => p.Id == fumbleSubmit.PlayId);

        if (play == null)
        {
            return null;
        }

        if (fumbleSubmit.FumbleCommittedById != null)
        {
            Player? fumbler = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == fumbleSubmit.FumbleCommittedById);
            if (fumbler == null)
            {
                return null;
            }
        }
        if (fumbleSubmit.FumbleForcedById != null)
        {
            Player? forcedBy = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == fumbleSubmit.FumbleForcedById);
            if (forcedBy == null)
            {
                return null;
            }
        }
        if (fumbleSubmit.FumbleRecoveredById != null)
        {
            Player? recoveredBy = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == fumbleSubmit.FumbleRecoveredById);
            if (recoveredBy == null)
            {
                return null;
            }
        }

        Fumble newFumble = new()
        {
            PlayId = play.Id,
            FumbleCommittedById = fumbleSubmit.FumbleCommittedById,
            FumbledAt = fumbleSubmit.FumbledAt,
            FumbleForcedById = fumbleSubmit.FumbleForcedById,
            FumbleRecoveredById = fumbleSubmit.FumbleRecoveredById,
            RecoveredAt = fumbleSubmit.FumbleRecoveredAt
        };

        _dbContext.Fumbles.Add(newFumble);
        await _dbContext.SaveChangesAsync();

        return newFumble;
    }
    public async Task<Fumble?> CreateFumbleAsync(Fumble newFumble)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == newFumble.PlayId);
        if (play == null)
        {
            return null;
        }

        if (newFumble.FumbleCommittedById != null)
        {
            Player? fumbler = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newFumble.FumbleCommittedById);
            if (fumbler == null)
            {
                return null;
            }
        }
        if (newFumble.FumbleForcedById != null)
        {
            Player? forcedBy = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newFumble.FumbleForcedById);
            if (forcedBy == null)
            {
                return null;
            }
        }
        if (newFumble.FumbleRecoveredById != null)
        {
            Player? recoveredBy = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newFumble.FumbleRecoveredById);
            if (recoveredBy == null)
            {
                return null;
            }
        }

        _dbContext.Fumbles.Add(newFumble);
        await _dbContext.SaveChangesAsync();

        return newFumble;
    }

    public Task<bool> DeleteFumbleAsync(int fumbleId)
    {
        throw new NotImplementedException();
    }

    public async Task<Fumble?> GetSingleFumbleAsync(int fumbleId)
    {
        return await _dbContext.Fumbles.AsNoTracking().SingleOrDefaultAsync(f => f.Id == fumbleId);
    }

    public Task<Fumble?> UpdateFumbleAsync(FumbleSubmitDTO fumbleSubmit)
    {
        throw new NotImplementedException();
    }
}