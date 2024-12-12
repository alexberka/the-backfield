using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Repositories
{
    public class PenaltyRepository : IPenaltyRepository
    {
        private readonly TheBackfieldDbContext _dbContext;

        public PenaltyRepository(TheBackfieldDbContext context)
        {
            _dbContext = context;
        }
        public Task<Penalty?> CreatePenaltyAsync(PenaltySubmitDTO penaltySubmit, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<string?> DeletePenaltyAsync(int penaltyId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Penalty>> GetAllPenaltiesAsync(int userId)
        {
            return await _dbContext.Penalties.Where(p => p.UserId == null || p.UserId == userId).ToListAsync();
        }

        public async Task<Penalty?> GetSinglePenaltyAsync(int penaltyId)
        {
            return await _dbContext.Penalties.AsNoTracking().SingleOrDefaultAsync(p => p.Id == penaltyId);
        }

        public Task<Penalty?> UpdatePenaltyAsync(PenaltySubmitDTO penaltySubmit)
        {
            throw new NotImplementedException();
        }
    }
}
