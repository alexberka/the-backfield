using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class InterceptionRepository : IInterceptionRepository
{
    private readonly TheBackfieldDbContext _dbContext;
    public InterceptionRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Interception?> CreateInterceptionAsync(PlaySubmitDTO playSubmit)
    {
        Play? play = await _dbContext.Plays.FindAsync(playSubmit.Id);
        if (play == null)
        {
            return null;
        }

        if (playSubmit.InterceptedById != null)
        {
            Player? defender = await _dbContext.Players.FindAsync(playSubmit.InterceptedById);
            if (defender == null)
            {
                return null;
            }
        }

        Interception newInterception = new()
        {
            PlayId = playSubmit.Id,
            InterceptedById = playSubmit.InterceptedById,
            InterceptedAt = playSubmit.InterceptedAt
        };

        _dbContext.Interceptions.Add(newInterception);
        await _dbContext.SaveChangesAsync();

        return newInterception;
    }

    public async Task<Interception?> CreateInterceptionAsync(Interception newInterception)
    {
        Play? play = await _dbContext.Plays.FindAsync(newInterception.PlayId);
        if (play == null)
        {
            return null;
        }

        if (newInterception.InterceptedById != null)
        {
            Player? defender = await _dbContext.Players.FindAsync(newInterception.InterceptedById);
            if (defender == null)
            {
                return null;
            }
        }

        _dbContext.Interceptions.Add(newInterception);
        await _dbContext.SaveChangesAsync();

        return newInterception;
    }

    public async Task<Interception?> UpdateInterceptionAsync(Interception interceptionUpdate)
    {
        Interception? interception = await _dbContext.Interceptions.FindAsync(interceptionUpdate.Id);
        if (interception == null)
        {
            return null;
        }

        if (interceptionUpdate.InterceptedById != null)
        {
            Player? defender = await _dbContext.Players.FindAsync(interceptionUpdate.InterceptedById);
            if (defender == null)
            {
                return null;
            }
        }

        interception.InterceptedById = interceptionUpdate.InterceptedById;
        interception.TeamId = interceptionUpdate.TeamId;
        interception.InterceptedAt = interceptionUpdate.InterceptedAt;
        interception.ReturnYardage = interceptionUpdate.ReturnYardage;

        await _dbContext.SaveChangesAsync();

        return interception;
    }

    public async Task<string?> DeleteInterceptionAsync(int interceptionId)
    {
        Interception? interception = await _dbContext.Interceptions.FindAsync(interceptionId);
        if (interception == null)
        {
            return "Invalid interception id";
        }

        _dbContext.Interceptions.Remove(interception);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<Interception?> GetSingleInterceptionAsync(int interceptionId)
    {
        return await _dbContext.Interceptions.AsNoTracking().SingleOrDefaultAsync(i => i.Id == interceptionId);
    }
}