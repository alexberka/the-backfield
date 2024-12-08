using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Repositories
{
    public class PlayRepository : IPlayRepository
    {
        private readonly TheBackfieldDbContext _dbContext;

        public PlayRepository(TheBackfieldDbContext context)
        {
            _dbContext = context;
        }

        public Task<Play?> CreatePlayAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }

        public Task<string?> DeletePlayAsync(int playId)
        {
            throw new NotImplementedException();
        }

        public async Task<Play?> GetSinglePlayAsync(int playId)
        {
            return await _dbContext.Plays
                .AsNoTracking()
                .Include(g => g.PrevPlay)
                .Include(p => p.Game)
                .Include(g => g.Pass)
                    .ThenInclude(p => p.Passer)
                .Include(g => g.Rush)
                .Include(g => g.Tacklers)
                .Include(g => g.PassDefenders)
                .Include(g => g.Kickoff)
                .Include(g => g.Punt)
                .Include(g => g.FieldGoal)
                .Include(g => g.ExtraPoint)
                .Include(g => g.Conversion)
                .Include(g => g.Fumbles)
                .Include(g => g.Interception)
                .Include(g => g.KickBlock)
                .Include(g => g.Laterals)
                .Include(g => g.Penalties)
                .SingleOrDefaultAsync(p => p.Id == playId);
        }

        public Task<Play?> UpdatePlayAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }
    }
}
