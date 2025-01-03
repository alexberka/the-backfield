using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class KickoffRepository : IKickoffRepository
{
    private readonly TheBackfieldDbContext _dbContext;
    public KickoffRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Kickoff?> CreateKickoffAsync(PlaySubmitDTO playSubmit)
    {
        Play? play = await _dbContext.Plays.FindAsync(playSubmit.Id);
        if (play == null)
        {
            return null;
        }

        if (playSubmit.KickerId != null)
        {
            Player? kicker = await _dbContext.Players.FindAsync(playSubmit.KickerId);
            if (kicker == null)
            {
                return null;
            }
        }

        if (playSubmit.KickReturnerId != null)
        {
            Player? returner = await _dbContext.Players.FindAsync(playSubmit.KickReturnerId);
            if (returner == null)
            {
                return null;
            }
        }

        Kickoff newKickoff = new()
        {
            PlayId = playSubmit.Id,
            KickerId = playSubmit.KickerId,
            ReturnerId = playSubmit.KickReturnerId,
            FieldedAt = playSubmit.KickFieldedAt,
            Touchback = playSubmit.KickTouchback
        };

        _dbContext.Kickoffs.Add(newKickoff);
        await _dbContext.SaveChangesAsync();

        return newKickoff;
    }

    public Task<bool> DeleteKickoffAsync(int kickoffId)
    {
        throw new NotImplementedException();
    }

    public async Task<Kickoff?> GetSingleKickoffAsync(int kickoffId)
    {
        return await _dbContext.Kickoffs.AsNoTracking().SingleOrDefaultAsync(k => k.Id == kickoffId);
    }

    public Task<Kickoff?> UpdateKickoffAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}