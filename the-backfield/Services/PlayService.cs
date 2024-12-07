using TheBackfield.DTOs;
using TheBackfield.Interfaces;

namespace TheBackfield.Services
{
    public class PlayService : IPlayService
    {
        public Task<PlayResponseDTO> CreatePlayAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }

        public Task<PlayResponseDTO> DeletePlayAsync(int playId, string sessionKey)
        {
            throw new NotImplementedException();
        }

        public Task<PlayResponseDTO> GetSinglePlayAsync(int playId, string sessionKey)
        {
            throw new NotImplementedException();
        }

        public Task<PlayResponseDTO> UpdatePlayAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }
    }
}
