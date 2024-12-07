using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class LateralRepository : ILateralRepository
{
    public Task<Lateral?> CreateLateralAsync(LateralSubmitDTO lateralSubmit, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteLateralAsync(int lateralId)
    {
        throw new NotImplementedException();
    }

    public Task<Lateral?> GetSingleLateralAsync(int lateralId)
    {
        throw new NotImplementedException();
    }

    public Task<Lateral?> UpdateLateralAsync(LateralSubmitDTO lateralSubmit)
    {
        throw new NotImplementedException();
    }
}