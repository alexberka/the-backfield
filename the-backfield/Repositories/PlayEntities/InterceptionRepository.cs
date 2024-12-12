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

    public Task<bool> DeleteInterceptionAsync(int interceptionId)
    {
        throw new NotImplementedException();
    }

    public async Task<Interception?> GetSingleInterceptionAsync(int interceptionId)
    {
        return await _dbContext.Interceptions.AsNoTracking().SingleOrDefaultAsync(i => i.Id == interceptionId);
    }

    public Task<Interception?> UpdateInterceptionAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}