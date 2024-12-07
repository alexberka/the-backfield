using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class InterceptionRepository : IInterceptionRepository
{
    public Task<Interception?> CreateInterceptionAsync(PlaySubmitDTO playSubmit, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteInterceptionAsync(int interceptionId)
    {
        throw new NotImplementedException();
    }

    public Task<Interception?> GetSingleInterceptionAsync(int interceptionId)
    {
        throw new NotImplementedException();
    }

    public Task<Interception?> UpdateInterceptionAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}