using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IKickoffRepository
{
    Task<Kickoff?> GetSingleKickoffAsync(int kickoffId);
    Task<Kickoff?> CreateKickoffAsync(PlaySubmitDTO playSubmit);
    Task<Kickoff?> CreateKickoffAsync(Kickoff newKickoff);
    Task<Kickoff?> UpdateKickoffAsync(Kickoff kickoffUpdate);
    Task<string?> DeleteKickoffAsync(int kickoffId);
}