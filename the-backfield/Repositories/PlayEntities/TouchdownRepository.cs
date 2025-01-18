using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities
{
    public class TouchdownRepository : ITouchdownRepository
    {
        private readonly TheBackfieldDbContext _dbContext;
        public TouchdownRepository(TheBackfieldDbContext context)
        {
            _dbContext = context;
        }
        public async Task<Touchdown?> CreateTouchdownAsync(PlaySubmitDTO playSubmit)
        {
            Play? play = await _dbContext.Plays
                .AsNoTracking()
                .Include(p => p.Game)
                .SingleOrDefaultAsync(p => p.Id == playSubmit.Id);
            if (play == null)
            {
                return null;
            }

            if (playSubmit.TouchdownPlayerId != null)
            {
                Player? player = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == playSubmit.TouchdownPlayerId);
                if (player == null || (player.TeamId != play.Game.HomeTeamId && player.TeamId != play.Game.AwayTeamId))
                {
                    return null;
                }
            }

            Touchdown newTouchdown = new()
            {
                PlayId = playSubmit.Id,
                PlayerId = playSubmit.TouchdownPlayerId
            };

            _dbContext.Touchdowns.Add(newTouchdown);
            await _dbContext.SaveChangesAsync();

            return newTouchdown;
        }
        public async Task<Touchdown?> CreateTouchdownAsync(Touchdown newTouchdown)
        {
            Play? play = await _dbContext.Plays
                .AsNoTracking()
                .Include(p => p.Game)
                .SingleOrDefaultAsync(p => p.Id == newTouchdown.PlayId);
            if (play == null || play.Game == null)
            {
                return null;
            }

            if (newTouchdown.PlayerId != null)
            {
                Player? player = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newTouchdown.PlayerId);
                if (player == null || !new int[] { play.Game.HomeTeamId, play.Game.AwayTeamId }.Contains(player.TeamId))
                {
                    return null;
                }
            }

            _dbContext.Touchdowns.Add(newTouchdown);
            await _dbContext.SaveChangesAsync();

            return newTouchdown;
        }

        public Task<bool> DeleteTouchdownAsync(int touchdownId)
        {
            throw new NotImplementedException();
        }

        public async Task<Touchdown?> GetSingleTouchdownAsync(int touchdownId)
        {
            return await _dbContext.Touchdowns.AsNoTracking().SingleOrDefaultAsync(t => t.Id == touchdownId);
        }

        public Task<Touchdown?> UpdateTouchdownAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }
    }
}
