using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface ITackleRepository
{
    Task<Tackle?> CreateTackleAsync(int playId, int tacklerId);
    Task<bool> DeleteTackleAsync(int tackleId);
}