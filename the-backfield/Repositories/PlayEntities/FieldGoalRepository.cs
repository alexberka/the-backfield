using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class FieldGoalRepository : IFieldGoalRepository
{
    public Task<FieldGoal?> CreateFieldGoalAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteFieldGoalAsync(int fieldGoalId)
    {
        throw new NotImplementedException();
    }

    public Task<FieldGoal?> GetSingleFieldGoalAsync(int fieldGoalId)
    {
        throw new NotImplementedException();
    }

    public Task<FieldGoal?> UpdateFieldGoalAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}