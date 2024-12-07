using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class ExtraPointRepository : IExtraPointRepository
{
    public Task<ExtraPoint?> CreateExtraPointAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteExtraPointAsync(int extraPointId)
    {
        throw new NotImplementedException();
    }

    public Task<ExtraPoint?> GetSingleExtraPointAsync(int extraPointId)
    {
        throw new NotImplementedException();
    }

    public Task<ExtraPoint?> UpdateExtraPointAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}