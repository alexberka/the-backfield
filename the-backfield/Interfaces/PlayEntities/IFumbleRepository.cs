using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IFumbleRepository
{
    Task<Fumble?> GetSingleFumbleAsync(int fumbleId);
    Task<Fumble?> CreateFumbleAsync(FumbleSubmitDTO fumbleSubmit, int userId);
    Task<Fumble?> UpdateFumbleAsync(FumbleSubmitDTO fumbleSubmit);
    Task<bool> DeleteFumbleAsync(int fumbleId);
}