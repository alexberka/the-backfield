using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Repositories;

namespace TheBackfield.Services
{
    public class PenaltyService : IPenaltyService
    {
        private readonly IPenaltyRepository _penaltyRepository;
        private readonly IUserRepository _userRepository;

        public PenaltyService(IPenaltyRepository penaltyRepository, IUserRepository userRepository)
        {
            _penaltyRepository = penaltyRepository;
            _userRepository = userRepository;
        }

        public Task<PenaltyResponseDTO> CreatePenaltyAsync(PenaltySubmitDTO penaltySubmit)
        {
            throw new NotImplementedException();
        }

        public Task<PenaltyResponseDTO> DeletePenaltyAsync(int penaltyId, string sessionKey)
        {
            throw new NotImplementedException();
        }

        public async Task<PenaltyResponseDTO> GetAllPenaltiesAsync(string sessionKey)
        {
            User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
            if (user == null)
            {
                return new PenaltyResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
            }

            return new PenaltyResponseDTO { Penalties = await _penaltyRepository.GetAllPenaltiesAsync(user.Id) };
        }

        public Task<PenaltyResponseDTO> GetSinglePenaltyAsync(int penaltyId, string sessionKey)
        {
            throw new NotImplementedException();
        }

        public Task<PenaltyResponseDTO> UpdatePenaltyAsync(PenaltySubmitDTO penaltySubmit)
        {
            throw new NotImplementedException();
        }
    }
}
