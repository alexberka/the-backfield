using TheBackfield.DTOs;

namespace TheBackfield.Interfaces;

public interface IPositionService
{
    Task<PositionResponseDTO> GetPositionsAsync();
}