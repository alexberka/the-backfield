using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IPlayPenaltyRepository
{
    Task<PlayPenalty?> GetSinglePlayPenaltyAsync(int playPenaltyId);
    Task<PlayPenalty?> CreatePlayPenaltyAsync(PlayPenaltySubmitDTO playPenaltySubmit, int userId);
    Task<PlayPenalty?> UpdatePlayPenaltyAsync(PlayPenaltySubmitDTO playPenaltySubmit);
    Task<bool> DeletePlayPenaltyAsync(int playPenaltyId);
}