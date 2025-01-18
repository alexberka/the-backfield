using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class RushRepository : IRushRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public RushRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Rush?> CreateRushAsync(PlaySubmitDTO playSubmit)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .Include(p => p.Game)
            .SingleOrDefaultAsync(p => p.Id == playSubmit.Id);

        if (play == null)
        {
            return null;
        }

        Player? rusher = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == playSubmit.RusherId);
        if (rusher == null || rusher.TeamId != play.TeamId)
        {
            return null;
        }

        Rush newRush = new()
        {
            PlayId = playSubmit.Id,
            RusherId = playSubmit.RusherId
        };

        _dbContext.Rushes.Add(newRush);
        await _dbContext.SaveChangesAsync();

        return newRush;
    }
    public async Task<Rush?> CreateRushAsync(Rush newRush)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == newRush.PlayId);
        if (play == null)
        {
            return null;
        }

        Player? rusher = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newRush.RusherId);
        if (rusher == null || rusher.TeamId != play.TeamId)
        {
            return null;
        }

        _dbContext.Rushes.Add(newRush);
        await _dbContext.SaveChangesAsync();

        return newRush;
    }

    public Task<bool> DeleteRushAsync(int rushId)
    {
        throw new NotImplementedException();
    }

    public async Task<Rush?> GetSingleRushAsync(int rushId)
    {
        return await _dbContext.Rushes.FindAsync(rushId);
    }

    public Task<Rush?> UpdateRushAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}