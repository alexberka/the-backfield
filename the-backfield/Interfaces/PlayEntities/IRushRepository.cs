using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IRushRepository
{
    Task<Rush?> GetSingleRushAsync(int rushId);
    Task<Rush?> CreateRushAsync(PlaySubmitDTO playSubmit);
    Task<Rush?> CreateRushAsync(Rush newRush);
    Task<Rush?> UpdateRushAsync(Rush rushUpdate);
    Task<string?> DeleteRushAsync(int rushId);
}