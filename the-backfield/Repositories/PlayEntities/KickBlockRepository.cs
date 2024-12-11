using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class KickBlockRepository : IKickBlockRepository
{
    private readonly TheBackfieldDbContext _dbContext;
    public KickBlockRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }
    public async Task<KickBlock?> CreateKickBlockAsync(PlaySubmitDTO playSubmit)
    {
        Play? play = await _dbContext.Plays.FindAsync(playSubmit.Id);
        if (play == null)
        {
            return null;
        }

        if (playSubmit.KickBlockedById != null)
        {
            Player? blocker = await _dbContext.Players.FindAsync(playSubmit.KickBlockedById);
            if (blocker != null)
            {
                return null;
            }
        }
        if (playSubmit.KickBlockRecoveredById != null)
        {
            Player? recovery = await _dbContext.Players.FindAsync(playSubmit.KickBlockRecoveredById);
            if (recovery != null)
            {
                return null;
            }
        }

        KickBlock newKickBlock = new()
        {
            PlayId = playSubmit.Id,
            BlockedById = playSubmit.KickBlockedById,
            RecoveredById = playSubmit.KickBlockRecoveredById,
            RecoveredAt = playSubmit.KickBlockRecoveredAt
        };

        _dbContext.KickBlocks.Add(newKickBlock);
        await _dbContext.SaveChangesAsync();

        return newKickBlock;
    }

    public Task<bool> DeleteKickBlockAsync(int kickBlockId)
    {
        throw new NotImplementedException();
    }

    public async Task<KickBlock?> GetSingleKickBlockAsync(int kickBlockId)
    {
        return await _dbContext.KickBlocks.AsNoTracking().SingleOrDefaultAsync(kb => kb.Id == kickBlockId);
    }

    public Task<KickBlock?> UpdateKickBlockAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}