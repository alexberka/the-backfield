using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IInterceptionRepository
{
    Task<Interception?> GetSingleInterceptionAsync(int interceptionId);
    Task<Interception?> CreateInterceptionAsync(PlaySubmitDTO playSubmit);
    Task<Interception?> UpdateInterceptionAsync(PlaySubmitDTO playSubmit);
    Task<bool> DeleteInterceptionAsync(int interceptionId);
}