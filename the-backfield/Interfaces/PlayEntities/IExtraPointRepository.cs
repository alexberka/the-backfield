using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IExtraPointRepository
{
    Task<ExtraPoint?> GetSingleExtraPointAsync(int extraPointId);
    Task<ExtraPoint?> CreateExtraPointAsync(PlaySubmitDTO playSubmit, int userId);
    Task<ExtraPoint?> UpdateExtraPointAsync(PlaySubmitDTO playSubmit);
    Task<bool> DeleteExtraPointAsync(int extraPointId);
}