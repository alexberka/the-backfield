using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IKickBlockRepository
{
    Task<KickBlock?> GetSingleKickBlockAsync(int kickBlockId);
    Task<KickBlock?> CreateKickBlockAsync(PlaySubmitDTO playSubmit, int userId);
    Task<KickBlock?> UpdateKickBlockAsync(PlaySubmitDTO playSubmit);
    Task<bool> DeleteKickBlockAsync(int kickBlockId);
}