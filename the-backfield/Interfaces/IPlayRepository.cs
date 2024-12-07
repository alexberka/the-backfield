using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces
{
    public interface IPlayRepository
    {
        Task<Play?> GetSinglePlayAsync(int playId);
        Task<Play?> CreatePlayAsync(PlaySubmitDTO playSubmit);
        Task<Play?> UpdatePlayAsync(PlaySubmitDTO playSubmit);
        Task<string?> DeletePlayAsync(int playId);
    }
}
