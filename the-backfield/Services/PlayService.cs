using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Utilities;

namespace TheBackfield.Services
{
    public class PlayService : IPlayService
    {
        private readonly IPlayRepository _playRepository;
        private readonly IUserRepository _userRepository;

        public PlayService(IPlayRepository playRepository, IUserRepository userRepository)
        {
            _playRepository = playRepository;
            _userRepository = userRepository;
        }

        public Task<PlayResponseDTO> CreatePlayAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }

        public Task<PlayResponseDTO> DeletePlayAsync(int playId, string sessionKey)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayResponseDTO> GetSinglePlayAsync(int playId, string sessionKey)
        {
            User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
            Play? play = await _playRepository.GetSinglePlayAsync(playId);
            return SessionKeyClient.VerifyAccess(sessionKey, user, play);
        }

        public Task<PlayResponseDTO> UpdatePlayAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }
    }
}
