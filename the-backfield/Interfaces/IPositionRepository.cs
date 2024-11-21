using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IPositionRepository
{
    Task<List<Position>> GetPositionsAsync();
    Task<Position?> GetSinglePosition(int positionId);
}