using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IKickoffRepository
{
    Task<Kickoff?> GetSingleKickoffAsync(int kickoffId);
    Task<Kickoff?> CreateKickoffAsync(PlaySubmitDTO playSubmit, int userId);
    Task<Kickoff?> UpdateKickoffAsync(PlaySubmitDTO playSubmit);
    Task<bool> DeleteKickoffAsync(int kickoffId);
}