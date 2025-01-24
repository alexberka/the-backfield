using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IPuntRepository
{
    Task<Punt?> GetSinglePuntAsync(int puntId);
    Task<Punt?> CreatePuntAsync(PlaySubmitDTO playSubmit);
    Task<Punt?> CreatePuntAsync(Punt newPunt);
    Task<Punt?> UpdatePuntAsync(Punt puntUpdate);
    Task<string?> DeletePuntAsync(int puntId);
}