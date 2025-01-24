using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface ITackleRepository
{
    Task<Tackle?> CreateTackleAsync(int playId, int tacklerId);
    Task<Tackle?> CreateTackleAsync(Tackle newTackle);
    Task<Tackle?> UpdateTackleAsync(Tackle tackleUpdate);
    Task<string?> DeleteTackleAsync(int tackleId);
}