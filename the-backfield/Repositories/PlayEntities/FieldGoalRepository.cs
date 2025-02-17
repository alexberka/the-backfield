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
    public async Task<FieldGoal?> CreateFieldGoalAsync(FieldGoal newFieldGoal)
    {
        Play? play = await _dbContext.Plays.FindAsync(newFieldGoal.PlayId);
        if (play == null)
        {
            return null;
        }

        if (newFieldGoal.KickerId != null)
        {
            Player? kicker = await _dbContext.Players.FindAsync(newFieldGoal.KickerId);
            if (kicker == null)
            {
                return null;
            }
        }

        _dbContext.FieldGoals.Add(newFieldGoal);
        await _dbContext.SaveChangesAsync();

        return newFieldGoal;
    }

    public async Task<FieldGoal?> UpdateFieldGoalAsync(FieldGoal fieldGoalUpdate)
    {
        FieldGoal? fieldGoal = await _dbContext.FieldGoals.FindAsync(fieldGoalUpdate.Id);
        if (fieldGoal == null)
        {
            return null;
        }

        if (fieldGoalUpdate.KickerId != null)
        {
            Player? kicker = await _dbContext.Players.FindAsync(fieldGoalUpdate.KickerId);
            if (kicker == null)
            {
                return null;
            }
        }

        fieldGoal.KickerId = fieldGoalUpdate.KickerId;
        fieldGoal.TeamId = fieldGoalUpdate.TeamId;
        fieldGoal.Good = fieldGoalUpdate.Good;
        fieldGoal.Fake = fieldGoalUpdate.Fake;
        fieldGoal.Distance = fieldGoalUpdate.Distance;

        await _dbContext.SaveChangesAsync();

        return fieldGoal;
    }

    public async Task<string?> DeleteFieldGoalAsync(int fieldGoalId)
    {
        FieldGoal? fieldGoal = await _dbContext.FieldGoals.FindAsync(fieldGoalId);
        if (fieldGoal == null)
        {
            return "Invalid field goal id";
        }

        _dbContext.FieldGoals.Remove(fieldGoal);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<FieldGoal?> GetSingleFieldGoalAsync(int fieldGoalId)
    {
        return await _dbContext.FieldGoals.AsNoTracking().SingleOrDefaultAsync(fg => fg.Id == fieldGoalId);
    }
}