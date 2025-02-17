using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IPassRepository
{
    Task<Pass?> GetSinglePassAsync(int passId);
    Task<Pass?> CreatePassAsync(PlaySubmitDTO playSubmit);
    Task<Pass?> CreatePassAsync(Pass newPass);
    Task<Pass?> UpdatePassAsync(Pass passUpdate);
    Task<string?> DeletePassAsync(int passId);
}