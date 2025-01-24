using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IKickBlockRepository
{
    Task<KickBlock?> GetSingleKickBlockAsync(int kickBlockId);
    Task<KickBlock?> CreateKickBlockAsync(PlaySubmitDTO playSubmit);
    Task<KickBlock?> CreateKickBlockAsync(KickBlock newKickBlock);
    Task<KickBlock?> UpdateKickBlockAsync(KickBlock kickBlockUpdate);
    Task<string?> DeleteKickBlockAsync(int kickBlockId);
}