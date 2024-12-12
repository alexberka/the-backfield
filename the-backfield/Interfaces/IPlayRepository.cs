using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces
{
    public interface IPlayRepository
    {
        Task<Play?> GetSinglePlayAsync(int playId);
        Task<List<Play>> GetScoringPlaysByGameAsync(int gameId);
        Task<List<Play>> GetCurrentDriveByGameAsync(int gameId);
        Task<Play?> GetLastPlayByGameAsync(int gameId);
        Task<Play?> CreatePlayAsync(PlaySubmitDTO playSubmit);
        Task<Play?> UpdatePlayAsync(PlaySubmitDTO playSubmit);
        Task<string?> DeletePlayAsync(int playId);
    }
}
