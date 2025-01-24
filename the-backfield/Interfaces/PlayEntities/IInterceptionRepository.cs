using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IInterceptionRepository
{
    Task<Interception?> GetSingleInterceptionAsync(int interceptionId);
    Task<Interception?> CreateInterceptionAsync(PlaySubmitDTO playSubmit);
    Task<Interception?> CreateInterceptionAsync(Interception newInterception);
    Task<Interception?> UpdateInterceptionAsync(Interception interceptionUpdate);
    Task<string?> DeleteInterceptionAsync(int interceptionId);
}