using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IFieldGoalRepository
{
    Task<FieldGoal?> GetSingleFieldGoalAsync(int fieldGoalId);
    Task<FieldGoal?> CreateFieldGoalAsync(PlaySubmitDTO playSubmit);
    Task<FieldGoal?> CreateFieldGoalAsync(FieldGoal newFieldGoal);
    Task<FieldGoal?> UpdateFieldGoalAsync(FieldGoal fieldGoalUpdate);
    Task<string?> DeleteFieldGoalAsync(int fieldGoalId);
}