using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IFumbleRepository
{
    Task<Fumble?> GetSingleFumbleAsync(int fumbleId);
    Task<Fumble?> CreateFumbleAsync(FumbleSubmitDTO fumbleSubmit);
    Task<Fumble?> CreateFumbleAsync(Fumble newFumble);
    Task<Fumble?> UpdateFumbleAsync(Fumble fumbleUpdate);
    Task<string?> DeleteFumbleAsync(int fumbleId);
}