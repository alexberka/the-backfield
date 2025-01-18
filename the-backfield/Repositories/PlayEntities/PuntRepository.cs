using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class PuntRepository : IPuntRepository
{
    private readonly TheBackfieldDbContext _dbContext;
    public PuntRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }
    public async Task<Punt?> CreatePuntAsync(PlaySubmitDTO playSubmit)
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

        Punt newPunt = new()
        {
            PlayId = playSubmit.Id,
            KickerId = playSubmit.KickerId,
            ReturnerId = playSubmit.KickReturnerId,
            FieldedAt = playSubmit.KickFieldedAt,
            FairCatch = playSubmit.KickFairCatch,
            Touchback = playSubmit.KickTouchback,
            Fake = playSubmit.KickFake
        };

        _dbContext.Punts.Add(newPunt);
        await _dbContext.SaveChangesAsync();

        return newPunt;
    }
    public async Task<Punt?> CreatePuntAsync(Punt newPunt)
    {
        Play? play = await _dbContext.Plays.FindAsync(newPunt.PlayId);
        if (play == null)
        {
            return null;
        }

        if (newPunt.KickerId != null)
        {
            Player? kicker = await _dbContext.Players.FindAsync(newPunt.KickerId);
            if (kicker == null)
            {
                return null;
            }
        }

        if (newPunt.ReturnerId != null)
        {
            Player? returner = await _dbContext.Players.FindAsync(newPunt.ReturnerId);
            if (returner == null)
            {
                return null;
            }
        }

        _dbContext.Punts.Add(newPunt);
        await _dbContext.SaveChangesAsync();

        return newPunt;
    }

    public Task<bool> DeletePuntAsync(int puntId)
    {
        throw new NotImplementedException();
    }

    public async Task<Punt?> GetSinglePuntAsync(int puntId)
    {
        return await _dbContext.Punts.AsNoTracking().SingleOrDefaultAsync(p => p.Id == puntId);
    }

    public Task<Punt?> UpdatePuntAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}