using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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

    public async Task<Fumble?> UpdateFumbleAsync(Fumble fumbleUpdate)
    {
        Fumble? fumble = await _dbContext.Fumbles.SingleOrDefaultAsync(p => p.Id == fumbleUpdate.Id);
        if (fumble == null)
        {
            return null;
        }

        if (fumbleUpdate.FumbleCommittedById != null)
        {
            Player? fumbler = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == fumbleUpdate.FumbleCommittedById);
            if (fumbler == null)
            {
                return null;
            }
        }
        if (fumbleUpdate.FumbleForcedById != null)
        {
            Player? forcedBy = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == fumbleUpdate.FumbleForcedById);
            if (forcedBy == null)
            {
                return null;
            }
        }
        if (fumbleUpdate.FumbleRecoveredById != null)
        {
            Player? recoveredBy = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == fumbleUpdate.FumbleRecoveredById);
            if (recoveredBy == null)
            {
                return null;
            }
        }

        fumble.FumbleCommittedById = fumbleUpdate.FumbleCommittedById;
        fumble.FumbleCommittedByTeamId = fumbleUpdate.FumbleCommittedByTeamId;
        fumble.FumbledAt = fumbleUpdate.FumbledAt;
        fumble.FumbleForcedById = fumbleUpdate.FumbleForcedById;
        fumble.FumbleForcedByTeamId = fumbleUpdate.FumbleForcedByTeamId;
        fumble.FumbleRecoveredById = fumbleUpdate.FumbleRecoveredById;
        fumble.FumbleRecoveredByTeamId = fumbleUpdate.FumbleRecoveredByTeamId;
        fumble.RecoveredAt = fumbleUpdate.RecoveredAt;
        fumble.LooseBallYardage = fumbleUpdate.LooseBallYardage;
        fumble.ReturnYardage = fumbleUpdate.ReturnYardage;
        fumble.YardageType = fumbleUpdate.YardageType;

        await _dbContext.SaveChangesAsync();

        return fumble;
    }

    public async Task<string?> DeleteFumbleAsync(int fumbleId)
    {
        Fumble? fumble = await _dbContext.Fumbles.FindAsync(fumbleId);
        if (fumble == null)
        {
            return "Invalid fumble id";
        }

        _dbContext.Fumbles.Remove(fumble);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<Fumble?> GetSingleFumbleAsync(int fumbleId)
    {
        return await _dbContext.Fumbles.AsNoTracking().SingleOrDefaultAsync(f => f.Id == fumbleId);
    }
}