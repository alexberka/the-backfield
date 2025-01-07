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

        public Task<ResponseDTO<Penalty>> CreatePenaltyAsync(PenaltySubmitDTO penaltySubmit)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO<Penalty>> DeletePenaltyAsync(int penaltyId, string sessionKey)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO<List<Penalty>>> GetAllPenaltiesAsync(string sessionKey)
        {
            User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
            if (user == null)
            {
                return new ResponseDTO<List<Penalty>> { Unauthorized = true, ErrorMessage = "Invalid session key" };
            }

            return new ResponseDTO<List<Penalty>> { Resource = await _penaltyRepository.GetAllPenaltiesAsync(user.Id) };
        }

        public Task<ResponseDTO<Penalty>> GetSinglePenaltyAsync(int penaltyId, string sessionKey)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO<Penalty>> UpdatePenaltyAsync(PenaltySubmitDTO penaltySubmit)
        {
            throw new NotImplementedException();
        }
    }
}
