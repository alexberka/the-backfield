using TheBackfield.DTOs.GameStream;

namespace TheBackfield.Interfaces
{
    public interface IGameStreamService
    {
        Task<GameStreamDTO?> GetGameStreamAsync(int gameId);
        Task<List<PlaySegmentDTO>> GetPlaySegmentsAsync(int playId);
        Task BroadcastGameStream(int gameId);
        Task BroadcastGameStreamByPlayId(int playId);
    }
}
