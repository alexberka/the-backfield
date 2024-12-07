using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class PassRepository : IPassRepository
{
    public Task<Pass?> CreatePassAsync(PlaySubmitDTO playSubmit, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeletePassAsync(int conversionId)
    {
        throw new NotImplementedException();
    }

    public Task<Pass?> GetSinglePassAsync(int conversionId)
    {
        throw new NotImplementedException();
    }

    public Task<Pass?> UpdatePassAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}