using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IExtraPointRepository
{
    Task<ExtraPoint?> GetSingleExtraPointAsync(int extraPointId);
    Task<ExtraPoint?> CreateExtraPointAsync(PlaySubmitDTO playSubmit);
    Task<ExtraPoint?> CreateExtraPointAsync(ExtraPoint newExtraPoint);
    Task<ExtraPoint?> UpdateExtraPointAsync(ExtraPoint extraPointUpdate);
    Task<string?> DeleteExtraPointAsync(int extraPointId);
}