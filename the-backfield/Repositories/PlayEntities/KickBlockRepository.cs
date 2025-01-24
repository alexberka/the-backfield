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
            if (blocker == null)
            {
                return null;
            }
        }
        if (playSubmit.KickBlockRecoveredById != null)
        {
            Player? recovery = await _dbContext.Players.FindAsync(playSubmit.KickBlockRecoveredById);
            if (recovery == null)
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

    public async Task<KickBlock?> CreateKickBlockAsync(KickBlock newKickBlock)
    {
        Play? play = await _dbContext.Plays.FindAsync(newKickBlock.PlayId);
        if (play == null)
        {
            return null;
        }

        if (newKickBlock.BlockedById != null)
        {
            Player? blocker = await _dbContext.Players.FindAsync(newKickBlock.BlockedById);
            if (blocker == null)
            {
                return null;
            }
        }
        if (newKickBlock.RecoveredById != null)
        {
            Player? recovery = await _dbContext.Players.FindAsync(newKickBlock.RecoveredById);
            if (recovery == null)
            {
                return null;
            }
        }

        _dbContext.KickBlocks.Add(newKickBlock);
        await _dbContext.SaveChangesAsync();

        return newKickBlock;
    }

    public async Task<KickBlock?> UpdateKickBlockAsync(KickBlock kickBlockUpdate)
    {
        KickBlock? kickBlock = await _dbContext.KickBlocks.FindAsync(kickBlockUpdate.Id);
        if (kickBlock == null)
        {
            return null;
        }

        if (kickBlockUpdate.BlockedById != null)
        {
            Player? blocker = await _dbContext.Players.FindAsync(kickBlockUpdate.BlockedById);
            if (blocker == null)
            {
                return null;
            }
        }
        if (kickBlockUpdate.RecoveredById != null)
        {
            Player? recovery = await _dbContext.Players.FindAsync(kickBlockUpdate.RecoveredById);
            if (recovery == null)
            {
                return null;
            }
        }

        kickBlock.BlockedById = kickBlockUpdate.BlockedById;
        kickBlock.BlockedByTeamId = kickBlockUpdate.BlockedByTeamId;
        kickBlock.RecoveredById = kickBlockUpdate.RecoveredById;
        kickBlock.RecoveredByTeamId = kickBlockUpdate.RecoveredByTeamId;
        kickBlock.RecoveredAt = kickBlockUpdate.RecoveredAt;
        kickBlock.LooseBallYardage = kickBlockUpdate.LooseBallYardage;
        kickBlock.ReturnYardage = kickBlockUpdate.ReturnYardage;

        await _dbContext.SaveChangesAsync();

        return kickBlock;
    }

    public async Task<string?> DeleteKickBlockAsync(int kickBlockId)
    {
        KickBlock? kickBlock = await _dbContext.KickBlocks.FindAsync(kickBlockId);
        if (kickBlock == null)
        {
            return "Invalid kick block id";
        }

        _dbContext.KickBlocks.Remove(kickBlock);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<KickBlock?> GetSingleKickBlockAsync(int kickBlockId)
    {
        return await _dbContext.KickBlocks.AsNoTracking().SingleOrDefaultAsync(kb => kb.Id == kickBlockId);
    }
}