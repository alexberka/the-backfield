using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Services;

public class PositionService : IPositionService
{
    private readonly IPositionRepository _positionRepository;

    public PositionService(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public async Task<ResponseDTO<List<Position>>> GetPositionsAsync()
    {
        return new ResponseDTO<List<Position>> { Resource = await _positionRepository.GetPositionsAsync() };
    }
}