using TheBackfield.DTOs;

namespace TheBackfield.Interfaces
{
    public interface IPlayService
    {
        Task<PlayResponseDTO> GetSinglePlayAsync(int playId, string sessionKey);
        Task<PlayResponseDTO> CreatePlayAsync(PlaySubmitDTO playSubmit);
        Task<PlayResponseDTO> UpdatePlayAsync(PlaySubmitDTO playSubmit);
        Task<PlayResponseDTO> DeletePlayAsync(int playId, string sessionKey);
    }
}
