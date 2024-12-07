using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class TackleRepository : ITackleRepository
{
    public Task<Tackle?> CreateTackleAsync(int playId, int tacklerId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteTackleAsync(int tackleId)
    {
        throw new NotImplementedException();
    }
}