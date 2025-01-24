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

    public async Task<Lateral?> CreateLateralAsync(Lateral newLateral)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == newLateral.PlayId);

        if (play == null)
        {
            return null;
        }

        Player? prevCarrier = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newLateral.PrevCarrierId);
        if (prevCarrier == null)
        {
            return null;
        }
        Player? newCarrier = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newLateral.NewCarrierId);
        if (newCarrier == null)
        {
            return null;
        }

        _dbContext.Laterals.Add(newLateral);
        await _dbContext.SaveChangesAsync();

        return newLateral;
    }

    public async Task<Lateral?> UpdateLateralAsync(Lateral lateralUpdate)
    {
        Lateral? lateral = await _dbContext.Laterals.SingleOrDefaultAsync(p => p.Id == lateralUpdate.Id);

        if (lateral == null)
        {
            return null;
        }

        if (lateralUpdate.PrevCarrier != null)
        {
            Player? prevCarrier = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == lateralUpdate.PrevCarrierId);
            if (prevCarrier == null)
            {
                return null;
            }
        }

        if (lateralUpdate.NewCarrier != null)
        {
            Player? newCarrier = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == lateralUpdate.NewCarrierId);
            if (newCarrier == null)
            {
                return null;
            }
        }

        lateral.PrevCarrierId = lateralUpdate.PrevCarrierId;
        lateral.NewCarrierId = lateralUpdate.NewCarrierId;
        lateral.TeamId = lateralUpdate.TeamId;
        lateral.PossessionAt = lateralUpdate.PossessionAt;
        lateral.CarriedTo = lateralUpdate.CarriedTo;
        lateral.Yardage = lateralUpdate.Yardage;
        lateral.YardageType = lateralUpdate.YardageType;

        await _dbContext.SaveChangesAsync();

        return lateral;
    }

    public async Task<string?> DeleteLateralAsync(int lateralId)
    {
        Lateral? lateral = await _dbContext.Laterals.FindAsync(lateralId);
        if (lateral == null)
        {
            return "Invalid lateral id";
        }

        _dbContext.Laterals.Remove(lateral);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<Lateral?> GetSingleLateralAsync(int lateralId)
    {
        return await _dbContext.Laterals.AsNoTracking().SingleOrDefaultAsync(l => l.Id == lateralId);
    }
}