using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IPositionService
{
    Task<ResponseDTO<List<Position>>> GetPositionsAsync();
}