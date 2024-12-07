using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class RushRepository : IRushRepository
{
    public Task<Rush?> CreateRushAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteRushAsync(int rushId)
    {
        throw new NotImplementedException();
    }

    public Task<Rush?> GetSingleRushAsync(int rushId)
    {
        throw new NotImplementedException();
    }

    public Task<Rush?> UpdateRushAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}