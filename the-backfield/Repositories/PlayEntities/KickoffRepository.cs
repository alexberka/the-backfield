using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class KickoffRepository : IKickoffRepository
{
    public Task<Kickoff?> CreateKickoffAsync(PlaySubmitDTO playSubmit, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteKickoffAsync(int kickoffId)
    {
        throw new NotImplementedException();
    }

    public Task<Kickoff?> GetSingleKickoffAsync(int kickoffId)
    {
        throw new NotImplementedException();
    }

    public Task<Kickoff?> UpdateKickoffAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}