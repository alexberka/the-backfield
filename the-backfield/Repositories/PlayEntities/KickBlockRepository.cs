using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class KickBlockRepository : IKickBlockRepository
{
    public Task<KickBlock?> CreateKickBlockAsync(PlaySubmitDTO playSubmit, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteKickBlockAsync(int kickBlockId)
    {
        throw new NotImplementedException();
    }

    public Task<KickBlock?> GetSingleKickBlockAsync(int kickBlockId)
    {
        throw new NotImplementedException();
    }

    public Task<KickBlock?> UpdateKickBlockAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}