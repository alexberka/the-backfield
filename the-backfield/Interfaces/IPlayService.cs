using TheBackfield.DTOs;
using TheBackfield.DTOs.GameStream;

namespace TheBackfield.Interfaces
{
    public interface IPlayService
    {
        Task<PlayResponseDTO> GetSinglePlayAsync(int playId, string sessionKey);
        Task<PlayResponseDTO> CreatePlayAsync(PlaySubmitDTO playSubmit);
        Task<PlayResponseDTO> UpdatePlayAsync(PlaySubmitDTO playSubmit);
        Task<List<PlaySegmentDTO>> GetPlaySegmentsAsync(int playId);
        Task<PlayResponseDTO> DeletePlayAsync(int playId, string sessionKey);
    }
}
