using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class ConversionRepository : IConversionRepository
{
    private readonly TheBackfieldDbContext _dbContext;
    public ConversionRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Conversion?> CreateConversionAsync(PlaySubmitDTO playSubmit)
    {
        Play? play = await _dbContext.Plays.FindAsync(playSubmit.Id);
        if (play == null)
        {
            return null;
        }

        if (playSubmit.ConversionPasserId != null)
        {
            Player? passer = await _dbContext.Players.FindAsync(playSubmit.ConversionPasserId);
            if (passer == null)
            {
                return null;
            }
        }
        if (playSubmit.ConversionReceiverId != null)
        {
            Player? receiver = await _dbContext.Players.FindAsync(playSubmit.ConversionReceiverId);
            if (receiver == null)
            {
                return null;
            }
        }
        if (playSubmit.ConversionRusherId != null)
        {
            Player? rusher = await _dbContext.Players.FindAsync(playSubmit.ConversionRusherId);
            if (rusher == null)
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

        Conversion newConversion = new()
        {
            PlayId = playSubmit.Id,
            PasserId = playSubmit.ConversionPasserId,
            ReceiverId = playSubmit.ConversionReceiverId,
            RusherId = playSubmit.ConversionRusherId,
            Good = playSubmit.ConversionGood,
            DefensiveConversion = playSubmit.DefensiveConversion,
            ReturnerId = playSubmit.ConversionReturnerId
        };

        _dbContext.Conversions.Add(newConversion);
        await _dbContext.SaveChangesAsync();

        return newConversion;
    }

    public async Task<Conversion?> CreateConversionAsync(Conversion newConversion)
    {
        Play? play = await _dbContext.Plays.FindAsync(newConversion.PlayId);
        if (play == null)
        {
            return null;
        }

        if (newConversion.PasserId != null)
        {
            Player? passer = await _dbContext.Players.FindAsync(newConversion.PasserId);
            if (passer == null)
            {
                return null;
            }
        }
        if (newConversion.ReceiverId != null)
        {
            Player? receiver = await _dbContext.Players.FindAsync(newConversion.ReceiverId);
            if (receiver == null)
            {
                return null;
            }
        }
        if (newConversion.RusherId != null)
        {
            Player? rusher = await _dbContext.Players.FindAsync(newConversion.RusherId);
            if (rusher == null)
            {
                return null;
            }
        }
        if (newConversion.ReturnerId != null)
        {
            Player? returner = await _dbContext.Players.FindAsync(newConversion.ReturnerId);
            if (returner == null)
            {
                return null;
            }
        }

        _dbContext.Conversions.Add(newConversion);
        await _dbContext.SaveChangesAsync();

        return newConversion;
    }

    public Task<bool> DeleteConversionAsync(int conversionId)
    {
        throw new NotImplementedException();
    }

    public async Task<Conversion?> GetSingleConversionAsync(int conversionId)
    {
        return await _dbContext.Conversions.AsNoTracking().SingleOrDefaultAsync(c => c.Id == conversionId);
    }

    public Task<Conversion?> UpdateConversionAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}