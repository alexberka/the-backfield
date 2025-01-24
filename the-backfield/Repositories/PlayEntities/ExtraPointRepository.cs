using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class ExtraPointRepository : IExtraPointRepository
{
    private readonly TheBackfieldDbContext _dbContext;
    public ExtraPointRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }
    public async Task<ExtraPoint?> CreateExtraPointAsync(PlaySubmitDTO playSubmit)
    {
        Play? play = await _dbContext.Plays.FindAsync(playSubmit.Id);
        if (play == null)
        {
            return null;
        }

        if (playSubmit.ExtraPointKickerId != null)
        {
            Player? kicker = await _dbContext.Players.FindAsync(playSubmit.ExtraPointKickerId);
            if (kicker == null)
            {
                return null;
            }
        }
        if (playSubmit.ConversionReturnerId != null)
        {
            Player? returner = await _dbContext.Players.FindAsync(playSubmit.ConversionReturnerId);
            if (returner == null)
            {
                return null;
            }
        }

        ExtraPoint newExtraPoint = new()
        {
            PlayId = playSubmit.Id,
            KickerId = playSubmit.ExtraPointKickerId,
            Good = playSubmit.ExtraPointGood,
            Fake = playSubmit.ExtraPointFake,
            DefensiveConversion = playSubmit.DefensiveConversion,
            ReturnerId = playSubmit.ConversionReturnerId
        };

        _dbContext.ExtraPoints.Add(newExtraPoint);
        await _dbContext.SaveChangesAsync();

        return newExtraPoint;
    }

    public async Task<ExtraPoint?> CreateExtraPointAsync(ExtraPoint newExtraPoint)
    {
        Play? play = await _dbContext.Plays.FindAsync(newExtraPoint.PlayId);
        if (play == null)
        {
            return null;
        }

        if (newExtraPoint.KickerId != null)
        {
            Player? kicker = await _dbContext.Players.FindAsync(newExtraPoint.KickerId);
            if (kicker == null)
            {
                return null;
            }
        }
        if (newExtraPoint.ReturnerId != null)
        {
            Player? returner = await _dbContext.Players.FindAsync(newExtraPoint.ReturnerId);
            if (returner == null)
            {
                return null;
            }
        }

        _dbContext.ExtraPoints.Add(newExtraPoint);
        await _dbContext.SaveChangesAsync();

        return newExtraPoint;
    }

    public async Task<ExtraPoint?> UpdateExtraPointAsync(ExtraPoint extraPointUpdate)
    {
        ExtraPoint? extraPoint = await _dbContext.ExtraPoints.FindAsync(extraPointUpdate.Id);
        if (extraPoint == null)
        {
            return null;
        }

        if (extraPointUpdate.KickerId != null)
        {
            Player? kicker = await _dbContext.Players.FindAsync(extraPointUpdate.KickerId);
            if (kicker == null)
            {
                return null;
            }
        }
        if (extraPointUpdate.ReturnerId != null)
        {
            Player? returner = await _dbContext.Players.FindAsync(extraPointUpdate.ReturnerId);
            if (returner == null)
            {
                return null;
            }
        }

        extraPoint.KickerId = extraPointUpdate.KickerId;
        extraPoint.TeamId = extraPointUpdate.TeamId;
        extraPoint.Good = extraPointUpdate.Good;
        extraPoint.Fake = extraPointUpdate.Fake;
        extraPoint.DefensiveConversion = extraPointUpdate.DefensiveConversion;
        extraPoint.ReturnerId = extraPointUpdate.ReturnerId;
        extraPoint.ReturnTeamId = extraPointUpdate.ReturnTeamId;

        await _dbContext.SaveChangesAsync();

        return extraPoint;
    }

    public async Task<string?> DeleteExtraPointAsync(int extraPointId)
    {
        ExtraPoint? extraPoint = await _dbContext.ExtraPoints.FindAsync(extraPointId);
        if (extraPoint == null)
        {
            return "Invalid extra point id";
        }

        _dbContext.ExtraPoints.Remove(extraPoint);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<ExtraPoint?> GetSingleExtraPointAsync(int extraPointId)
    {
        return await _dbContext.ExtraPoints.AsNoTracking().SingleOrDefaultAsync(c => c.Id == extraPointId);
    }
}