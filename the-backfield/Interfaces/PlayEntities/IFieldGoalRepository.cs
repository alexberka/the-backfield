using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IFieldGoalRepository
{
    Task<FieldGoal?> GetSingleFieldGoalAsync(int fieldGoalId);
    Task<FieldGoal?> CreateFieldGoalAsync(PlaySubmitDTO playSubmit, int userId);
    Task<FieldGoal?> UpdateFieldGoalAsync(PlaySubmitDTO playSubmit);
    Task<bool> DeleteFieldGoalAsync(int fieldGoalId);
}