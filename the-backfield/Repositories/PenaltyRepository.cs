using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Repositories
{
    public class PenaltyRepository : IPenaltyRepository
    {
        public Task<Penalty?> CreatePenaltyAsync(PenaltySubmitDTO penaltySubmit, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<string?> DeletePenaltyAsync(int penaltyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Penalty>> GetAllPenaltiesAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Penalty?> GetSinglePenaltyAsync(int penaltyId)
        {
            throw new NotImplementedException();
        }

        public Task<Penalty?> UpdatePenaltyAsync(PenaltySubmitDTO penaltySubmit)
        {
            throw new NotImplementedException();
        }
    }
}
