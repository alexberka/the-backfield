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

    public async Task<Punt?> UpdatePuntAsync(Punt puntUpdate)
    {
        Punt? punt = await _dbContext.Punts.FindAsync(puntUpdate.Id);
        if (punt == null)
        {
            return null;
        }

        if (puntUpdate.KickerId != null)
        {
            Player? kicker = await _dbContext.Players.FindAsync(puntUpdate.KickerId);
            if (kicker == null)
            {
                return null;
            }
        }

        if (puntUpdate.ReturnerId != null)
        {
            Player? returner = await _dbContext.Players.FindAsync(puntUpdate.ReturnerId);
            if (returner == null)
            {
                return null;
            }
        }

        punt.KickerId = puntUpdate.KickerId;
        punt.TeamId = puntUpdate.TeamId;
        punt.ReturnerId = puntUpdate.ReturnerId;
        punt.ReturnTeamId = puntUpdate.ReturnTeamId;
        punt.FieldedAt = puntUpdate.FieldedAt;
        punt.FairCatch = puntUpdate.FairCatch;
        punt.Touchback = puntUpdate.Touchback;
        punt.Fake = puntUpdate.Fake;
        punt.Distance = puntUpdate.Distance;
        punt.ReturnYardage = puntUpdate.ReturnYardage;

        await _dbContext.SaveChangesAsync();

        return punt;
    }

    public async Task<string?> DeletePuntAsync(int puntId)
    {
        Punt? punt = await _dbContext.Punts.FindAsync(puntId);
        if (punt == null)
        {
            return "Invalid punt id";
        }

        _dbContext.Punts.Remove(punt);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<Punt?> GetSinglePuntAsync(int puntId)
    {
        return await _dbContext.Punts.AsNoTracking().SingleOrDefaultAsync(p => p.Id == puntId);
    }
}