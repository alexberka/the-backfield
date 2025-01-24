using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities
{
    public interface ISafetyRepository
    {
        Task<Safety?> GetSingleSafetyAsync(int safetyId);
        Task<Safety?> CreateSafetyAsync(PlaySubmitDTO playSubmit);
        Task<Safety?> CreateSafetyAsync(Safety newSafety);
        Task<Safety?> UpdateSafetyAsync(Safety safetyUpdate);
        Task<string?> DeleteSafetyAsync(int safetyId);
    }
}
