using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class PuntRepository : IPuntRepository
{
    public Task<Punt?> CreatePuntAsync(PlaySubmitDTO playSubmit, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeletePuntAsync(int puntId)
    {
        throw new NotImplementedException();
    }

    public Task<Punt?> GetSinglePuntAsync(int puntId)
    {
        throw new NotImplementedException();
    }

    public Task<Punt?> UpdatePuntAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}