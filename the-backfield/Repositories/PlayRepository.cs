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

        public async Task<Play?> UpdatePlayAsync(PlaySubmitDTO playUpdate)
        {
            Play? play = await _dbContext.Plays.SingleOrDefaultAsync(p => p.Id == playUpdate.Id);
            if (play == null)
            {
                return null;
            }

            Team? team = await _dbContext.Teams.AsNoTracking().SingleOrDefaultAsync(t => t.Id == playUpdate.TeamId);
            if (team == null)
            {
                return null;
            }

            play.TeamId = playUpdate.TeamId;
            play.FieldPositionStart = playUpdate.FieldPositionStart;
            play.FieldPositionEnd = playUpdate.FieldPositionEnd;
            play.Down = playUpdate.Down;
            play.ToGain = playUpdate.ToGain;
            play.ClockStart = playUpdate.ClockStart;
            play.ClockEnd = playUpdate.ClockEnd;
            play.GamePeriod = playUpdate.GamePeriod;
            play.Notes = playUpdate.Notes;

            await _dbContext.SaveChangesAsync();

            return play;
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
                    .ThenInclude(g => g.HomeTeam)
                .Include(p => p.Game)
                    .ThenInclude(g => g.AwayTeam)
                .Include(p => p.Pass)
                    .ThenInclude(p => p.Passer)
                .Include(p => p.Pass)
                    .ThenInclude(p => p.Receiver)
                .Include(p => p.Rush)
                    .ThenInclude(r => r.Rusher)
                .Include(p => p.Tacklers)
                    .ThenInclude(p => p.Tackler)
                .Include(p => p.PassDefenders)
                    .ThenInclude(p => p.Defender)
                .Include(p => p.Kickoff)
                    .ThenInclude(k => k.Kicker)
                .Include(p => p.Kickoff)
                    .ThenInclude(k => k.Returner)
                .Include(p => p.Punt)
                    .ThenInclude(p => p.Kicker)
                .Include(p => p.Punt)
                    .ThenInclude(p => p.Returner)
                .Include(p => p.FieldGoal)
                    .ThenInclude(fg => fg.Kicker)
                .Include(p => p.Touchdown)
                    .ThenInclude(fg => fg.Player)
                .Include(p => p.ExtraPoint)
                    .ThenInclude(ep => ep.Kicker)
                .Include(p => p.ExtraPoint)
                    .ThenInclude(ep => ep.Returner)
                .Include(p => p.Conversion)
                    .ThenInclude(c => c.Passer)
                .Include(p => p.Conversion)
                    .ThenInclude(c => c.Receiver)
                .Include(p => p.Conversion)
                    .ThenInclude(c => c.Rusher)
                .Include(p => p.Conversion)
                    .ThenInclude(c => c.Returner)
                .Include(p => p.Safety)
                    .ThenInclude(s => s.CedingPlayer)
                .Include(p => p.Fumbles)
                    .ThenInclude(f => f.FumbleCommittedBy)
                .Include(p => p.Fumbles)
                    .ThenInclude(f => f.FumbleForcedBy)
                .Include(p => p.Fumbles)
                    .ThenInclude(f => f.FumbleRecoveredBy)
                .Include(p => p.Interception)
                    .ThenInclude(i => i.InterceptedBy)
                .Include(p => p.KickBlock)
                    .ThenInclude(kb => kb.BlockedBy)
                .Include(p => p.KickBlock)
                    .ThenInclude(kb => kb.RecoveredBy)
                .Include(p => p.Laterals)
                    .ThenInclude(l => l.PrevCarrier)
                .Include(p => p.Laterals)
                    .ThenInclude(l => l.NewCarrier)
                .Include(p => p.Penalties)
                    .ThenInclude(pp => pp.Penalty)
                .Include(p => p.Penalties)
                    .ThenInclude(pp => pp.Player)
                .SingleOrDefaultAsync(p => p.Id == playId);
        }

        public async Task<List<Play>> GetScoringPlaysByGameAsync(int gameId)
        {
            return await _dbContext.Plays
                .AsNoTracking()
                .Include(p => p.Game)
                .Include(p => p.Pass)
                    .ThenInclude(p => p.Passer)
                .Include(p => p.Pass)
                    .ThenInclude(p => p.Receiver)
                .Include(p => p.Rush)
                    .ThenInclude(r => r.Rusher)
                .Include(p => p.Kickoff)
                    .ThenInclude(k => k.Kicker)
                .Include(p => p.Kickoff)
                    .ThenInclude(k => k.Returner)
                .Include(p => p.Punt)
                    .ThenInclude(p => p.Kicker)
                .Include(p => p.Punt)
                    .ThenInclude(p => p.Returner)
                .Include(p => p.FieldGoal)
                .Include(p => p.ExtraPoint)
                .Include(p => p.Conversion)
                .Include(p => p.Fumbles)
                .Include(p => p.Interception)
                .Include(p => p.KickBlock)
                .Include(p => p.Laterals)
                .Include(p => p.Penalties)
                .Where(p => p.GameId == gameId)
                .Where(p => Math.Abs(p.FieldPositionEnd ?? 0) == 50 && !p.Penalties.Any(pe => pe.NoPlay == true && pe.Enforced == true))
                .ToListAsync();
        }

        public Task<List<Play>> GetCurrentDriveByGameAsync(int gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<Play?> GetLastPlayByGameAsync(int gameId)
        {
            List<Play> gamePlays = await _dbContext.Plays
                .AsNoTracking()
                .Include(p => p.Pass)
                    .ThenInclude(p => p.Passer)
                .Include(p => p.Pass)
                    .ThenInclude(p => p.Receiver)
                .Include(p => p.Rush)
                    .ThenInclude(r => r.Rusher)
                .Include(p => p.Kickoff)
                .Include(p => p.Punt)
                .Include(p => p.FieldGoal)
                .Include(p => p.ExtraPoint)
                .Include(p => p.Conversion)
                .Include(p => p.Fumbles)
                .Include(p => p.Interception)
                .Include(p => p.KickBlock)
                .Include(p => p.Laterals)
                .Include(p => p.Penalties)
                .Where(p => p.GameId == gameId)
                .ToListAsync();

            Play? lastPlay = gamePlays.SingleOrDefault(p => !gamePlays.Any(gp => gp.PrevPlayId == p.Id));

            return lastPlay;
        }
    }
}
