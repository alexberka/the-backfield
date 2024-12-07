using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class FumbleRepository : IFumbleRepository
{
    public Task<Fumble?> CreateFumbleAsync(FumbleSubmitDTO fumbleSubmit, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteFumbleAsync(int fumbleId)
    {
        throw new NotImplementedException();
    }

    public Task<Fumble?> GetSingleFumbleAsync(int fumbleId)
    {
        throw new NotImplementedException();
    }

    public Task<Fumble?> UpdateFumbleAsync(FumbleSubmitDTO fumbleSubmit)
    {
        throw new NotImplementedException();
    }
}