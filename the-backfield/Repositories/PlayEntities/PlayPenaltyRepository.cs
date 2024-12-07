using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class PlayPenaltyRepository : IPlayPenaltyRepository
{
    public Task<PlayPenalty?> CreatePlayPenaltyAsync(PlayPenaltySubmitDTO playPenaltySubmit, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeletePlayPenaltyAsync(int playPenaltyId)
    {
        throw new NotImplementedException();
    }

    public Task<PlayPenalty?> GetSinglePlayPenaltyAsync(int playPenaltyId)
    {
        throw new NotImplementedException();
    }

    public Task<PlayPenalty?> UpdatePlayPenaltyAsync(PlayPenaltySubmitDTO playPenaltySubmit)
    {
        throw new NotImplementedException();
    }
}