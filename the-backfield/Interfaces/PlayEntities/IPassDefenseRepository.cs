using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IPassDefenseRepository
{
    Task<PassDefense?> CreatePassDefenseAsync(int playId, int defenderId);
    Task<PassDefense?> CreatePassDefenseAsync(PassDefense newPassDefense);
    Task<bool> DeletePassDefenseAsync(int passDefenseId);
}