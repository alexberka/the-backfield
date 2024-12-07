using TheBackfield.DTOs;
using TheBackfield.Interfaces;

namespace TheBackfield.Services
{
    public class PenaltyService : IPenaltyService
    {
        public Task<PenaltyResponseDTO> CreatePenaltyAsync(PenaltySubmitDTO penaltySubmit)
        {
            throw new NotImplementedException();
        }

        public Task<PenaltyResponseDTO> DeletePenaltyAsync(int penaltyId, string sessionKey)
        {
            throw new NotImplementedException();
        }

        public Task<PenaltyResponseDTO> GetAllPenaltiesAsync(string sessionKey)
        {
            throw new NotImplementedException();
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
