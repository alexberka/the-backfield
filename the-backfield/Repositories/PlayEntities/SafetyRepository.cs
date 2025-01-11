using Microsoft.EntityFrameworkCore;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities
{
    public class SafetyRepository : ISafetyRepository
    {
        private readonly TheBackfieldDbContext _dbContext;
        public SafetyRepository(TheBackfieldDbContext context)
        {
            _dbContext = context;
        }
        public async Task<Safety?> CreateSafetyAsync(PlaySubmitDTO playSubmit)
        {
            Play? play = await _dbContext.Plays
                .AsNoTracking()
                .Include(p => p.Game)
                .SingleOrDefaultAsync(p => p.Id == playSubmit.Id);
            if (play == null)
            {
                return null;
            }

            if (playSubmit.CedingPlayerId != null)
            {
                Player? ceding = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == playSubmit.CedingPlayerId);
                if (ceding == null || (ceding.TeamId != play.Game.HomeTeamId && ceding.TeamId != play.Game.AwayTeamId))
                {
                    return null;
                }
            }

            Safety newSafety = new()
            {
                PlayId = playSubmit.Id,
                CedingPlayerId = playSubmit.CedingPlayerId
            };

            _dbContext.Safeties.Add(newSafety);
            await _dbContext.SaveChangesAsync();

            return newSafety;
        }

        public Task<bool> DeleteSafetyAsync(int safetyId)
        {
            throw new NotImplementedException();
        }

        public async Task<Safety?> GetSingleSafetyAsync(int safetyId)
        {
            return await _dbContext.Safeties.AsNoTracking().SingleOrDefaultAsync(s => s.Id == safetyId);
        }

        public Task<Safety?> UpdateSafetyAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }
    }
}
