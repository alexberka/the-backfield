using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IPassRepository
{
    Task<Pass?> GetSinglePassAsync(int conversionId);
    Task<Pass?> CreatePassAsync(PlaySubmitDTO playSubmit, int userId);
    Task<Pass?> UpdatePassAsync(PlaySubmitDTO playSubmit);
    Task<bool> DeletePassAsync(int conversionId);
}