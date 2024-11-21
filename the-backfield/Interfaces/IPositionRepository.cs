using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IPositionRepository
{
    Task<List<Position>> GetPositionsAsync();
    Task<Position?> GetSinglePositionAsync(int positionId);
}