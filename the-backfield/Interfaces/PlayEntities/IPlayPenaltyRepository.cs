using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IPlayPenaltyRepository
{
    Task<PlayPenalty?> GetSinglePlayPenaltyAsync(int playPenaltyId);
    Task<PlayPenalty?> CreatePlayPenaltyAsync(PlayPenaltySubmitDTO playPenaltySubmit);
    Task<PlayPenalty?> CreatePlayPenaltyAsync(PlayPenalty newPlayPenalty);
    Task<PlayPenalty?> UpdatePlayPenaltyAsync(PlayPenalty playPenaltyUpdate);
    Task<string?> DeletePlayPenaltyAsync(int playPenaltyId);
}