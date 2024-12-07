using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class PassDefenseRepository : IPassDefenseRepository
{
    public Task<PassDefense?> CreatePassDefenseAsync(int playId, int defenderId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeletePassDefenseAsync(int passDefenseId)
    {
        throw new NotImplementedException();
    }
}