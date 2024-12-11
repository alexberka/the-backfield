using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class FieldGoalRepository : IFieldGoalRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public FieldGoalRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public async Task<FieldGoal?> CreateFieldGoalAsync(PlaySubmitDTO playSubmit)
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

        FieldGoal newFieldGoal = new()
        {
            PlayId = playSubmit.Id,
            KickerId = playSubmit.KickerId,
            Good = playSubmit.KickGood,
            Fake = playSubmit.KickFake
        };

        _dbContext.FieldGoals.Add(newFieldGoal);
        await _dbContext.SaveChangesAsync();

        return newFieldGoal;
    }

    public Task<bool> DeleteFieldGoalAsync(int fieldGoalId)
    {
        throw new NotImplementedException();
    }

    public async Task<FieldGoal?> GetSingleFieldGoalAsync(int fieldGoalId)
    {
        return await _dbContext.FieldGoals.AsNoTracking().SingleOrDefaultAsync(fg => fg.Id == fieldGoalId);
    }

    public Task<FieldGoal?> UpdateFieldGoalAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}