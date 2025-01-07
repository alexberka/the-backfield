using TheBackfield.DTOs;
using TheBackfield.DTOs.GameStream;
using TheBackfield.Models;

namespace TheBackfield.Interfaces
{
    public interface IPlayService
    {
        Task<ResponseDTO<Play>> GetSinglePlayAsync(int playId, string sessionKey);
        Task<ResponseDTO<Play>> CreatePlayAsync(PlaySubmitDTO playSubmit);
        Task<ResponseDTO<Play>> UpdatePlayAsync(PlaySubmitDTO playSubmit);
        Task<List<PlaySegmentDTO>> GetPlaySegmentsAsync(int playId);
        Task<ResponseDTO<Play>> DeletePlayAsync(int playId, string sessionKey);
    }
}
