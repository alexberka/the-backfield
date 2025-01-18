using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface ILateralRepository
{
    Task<Lateral?> GetSingleLateralAsync(int lateralId);
    Task<Lateral?> CreateLateralAsync(LateralSubmitDTO lateralSubmit);
    Task<Lateral?> CreateLateralAsync(Lateral newLateral);
    Task<Lateral?> UpdateLateralAsync(LateralSubmitDTO lateralSubmit);
    Task<bool> DeleteLateralAsync(int lateralId);
}