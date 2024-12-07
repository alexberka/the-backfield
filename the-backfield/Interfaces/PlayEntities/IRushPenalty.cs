using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IRushRepository
{
    Task<Rush?> GetSingleRushAsync(int rushId);
    Task<Rush?> CreateRushAsync(PlaySubmitDTO playSubmit);
    Task<Rush?> UpdateRushAsync(PlaySubmitDTO playSubmit);
    Task<bool> DeleteRushAsync(int rushId);
}