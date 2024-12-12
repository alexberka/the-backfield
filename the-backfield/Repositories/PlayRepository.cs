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

        public async Task<Play?> CreatePlayAsync(PlaySubmitDTO playSubmit)
        {
            Play? playAfter = await _dbContext.Plays.SingleOrDefaultAsync(p => p.PrevPlayId == playSubmit.PrevPlayId && p.GameId == playSubmit.GameId);

            Play newPlay = new()
            {
                PrevPlayId = playSubmit.PrevPlayId,
                GameId = playSubmit.GameId,
                TeamId = playSubmit.TeamId,
                FieldPositionStart = playSubmit.FieldPositionStart,
                FieldPositionEnd = playSubmit.FieldPositionEnd,
                Down = playSubmit.Down,
                ToGain = playSubmit.ToGain,
                ClockStart = playSubmit.ClockStart,
                ClockEnd = playSubmit.ClockEnd,
                GamePeriod = playSubmit.GamePeriod,
                Notes = playSubmit.Notes
            };

            _dbContext.Plays.Add(newPlay);

            await _dbContext.SaveChangesAsync();

            if (playAfter != null)
            {
                playAfter.PrevPlayId = newPlay.Id;
            }

            await _dbContext.SaveChangesAsync();

            return newPlay;
        }

        public async Task<string?> DeletePlayAsync(int playId)
        {
            Play? playToDelete = await _dbContext.Plays.FindAsync(playId);

            if (playToDelete == null)
            {
                return "Invalid play id";
            }

            Play? subsequentPlay = await _dbContext.Plays.SingleOrDefaultAsync(p => p.PrevPlayId == playId);

            if (subsequentPlay != null)
            {
                subsequentPlay.PrevPlayId = playToDelete.PrevPlayId;
            }

            _dbContext.Plays.Remove(playToDelete);
            await _dbContext.SaveChangesAsync();

            return null;
        }

        public async Task<Play?> GetSinglePlayAsync(int playId)
        {
            return await _dbContext.Plays
                .AsNoTracking()
                .Include(p => p.PrevPlay)
                .Include(p => p.Game)
                .Include(p => p.Pass)
                    .ThenInclude(p => p.Passer)
                .Include(p => p.Rush)
                .Include(p => p.Tacklers)
                .Include(p => p.PassDefenders)
                .Include(p => p.Kickoff)
                .Include(p => p.Punt)
                .Include(p => p.FieldGoal)
                .Include(p => p.Touchdown)
                .Include(p => p.ExtraPoint)
                .Include(p => p.Conversion)
                .Include(p => p.Safety)
                .Include(p => p.Fumbles)
                .Include(p => p.Interception)
                .Include(p => p.KickBlock)
                .Include(p => p.Laterals)
                .Include(p => p.Penalties)
                .SingleOrDefaultAsync(p => p.Id == playId);
        }

        public Task<Play?> UpdatePlayAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }
    }
}
