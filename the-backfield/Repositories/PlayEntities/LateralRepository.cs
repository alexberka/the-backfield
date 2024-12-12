using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class LateralRepository : ILateralRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public LateralRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }
    public async Task<Lateral?> CreateLateralAsync(LateralSubmitDTO lateralSubmit)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .Include(p => p.Game)
            .SingleOrDefaultAsync(p => p.Id == lateralSubmit.PlayId);

        if (play == null)
        {
            return null;
        }

        Player? prevCarrier = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == lateralSubmit.PrevCarrierId);
        if (prevCarrier == null)
        {
            return null;
        }
        Player? newCarrier = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == lateralSubmit.NewCarrierId);
        if (newCarrier == null)
        {
            return null;
        }

        Lateral newLateral = new()
        {
            PlayId = play.Id,
            PrevCarrierId = lateralSubmit.PrevCarrierId,
            NewCarrierId = lateralSubmit.NewCarrierId,
            PossessionAt = lateralSubmit.PossessionAt,
            CarriedTo = lateralSubmit.CarriedTo
        };

        _dbContext.Laterals.Add(newLateral);
        await _dbContext.SaveChangesAsync();

        return newLateral;
    }

    public Task<bool> DeleteLateralAsync(int lateralId)
    {
        throw new NotImplementedException();
    }

    public async Task<Lateral?> GetSingleLateralAsync(int lateralId)
    {
        return await _dbContext.Laterals.AsNoTracking().SingleOrDefaultAsync(l => l.Id == lateralId);
    }

    public Task<Lateral?> UpdateLateralAsync(LateralSubmitDTO lateralSubmit)
    {
        throw new NotImplementedException();
    }
}