using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IPassDefenseRepository
{
    Task<PassDefense?> CreatePassDefenseAsync(int playId, int defenderId, int userId);
    Task<bool> DeletePassDefenseAsync(int passDefenseId);
}