using TheBackfield.DTOs;
using TheBackfield.Interfaces;

namespace TheBackfield.Services;

public class PositionService : IPositionService
{
    private readonly IPositionRepository _positionRepository;

    public PositionService(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public async Task<PositionResponseDTO> GetPositionsAsync()
    {
        return new PositionResponseDTO { Positions = await _positionRepository.GetPositionsAsync() };
    }
}