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

    public Task<bool> DeleteExtraPointAsync(int extraPointId)
    {
        throw new NotImplementedException();
    }

    public async Task<ExtraPoint?> GetSingleExtraPointAsync(int extraPointId)
    {
        return await _dbContext.ExtraPoints.AsNoTracking().SingleOrDefaultAsync(c => c.Id == extraPointId);
    }

    public Task<ExtraPoint?> UpdateExtraPointAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}