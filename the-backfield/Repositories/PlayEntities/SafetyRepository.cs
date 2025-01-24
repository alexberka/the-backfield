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

        public async Task<Safety?> CreateSafetyAsync(Safety newSafety)
        {
            Play? play = await _dbContext.Plays
                .AsNoTracking()
                .Include(p => p.Game)
                .SingleOrDefaultAsync(p => p.Id == newSafety.PlayId);
            if (play == null || play.Game == null)
            {
                return null;
            }

            if (newSafety.CedingPlayerId != null)
            {
                Player? ceding = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newSafety.CedingPlayerId);
                if (ceding == null || !new int[] { play.Game.HomeTeamId, play.Game.AwayTeamId }.Contains(ceding.TeamId))
                {
                    return null;
                }
            }

            _dbContext.Safeties.Add(newSafety);
            await _dbContext.SaveChangesAsync();

            return newSafety;
        }

        public async Task<Safety?> UpdateSafetyAsync(Safety safetyUpdate)
        {
            Safety? safety = await _dbContext.Safeties.SingleOrDefaultAsync(p => p.Id == safetyUpdate.Id);
            if (safety == null)
            {
                return null;
            }

            if (safetyUpdate.CedingPlayerId != null)
            {
                Player? ceding = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == safetyUpdate.CedingPlayerId);
                if (ceding == null)
                {
                    return null;
                }
            }

            safety.CedingPlayerId = safetyUpdate.CedingPlayerId;
            safety.CedingTeamId = safetyUpdate.CedingTeamId;

            await _dbContext.SaveChangesAsync();

            return safety;
        }

        public async Task<string?> DeleteSafetyAsync(int safetyId)
        {
            Safety? safety = await _dbContext.Safeties.FindAsync(safetyId);
            if (safety == null)
            {
                return "Invalid safety id";
            }

            _dbContext.Safeties.Remove(safety);
            await _dbContext.SaveChangesAsync();
            return null;
        }

        public async Task<Safety?> GetSingleSafetyAsync(int safetyId)
        {
            return await _dbContext.Safeties.AsNoTracking().SingleOrDefaultAsync(s => s.Id == safetyId);
        }
    }
}
