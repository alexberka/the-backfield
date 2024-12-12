using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs.PlayEntities;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class PlayPenaltyRepository : IPlayPenaltyRepository
{
    private readonly TheBackfieldDbContext _dbContext;
    public PlayPenaltyRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public async Task<PlayPenalty?> CreatePlayPenaltyAsync(PlayPenaltySubmitDTO playPenaltySubmit)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .Include(p => p.Game)
            .SingleOrDefaultAsync(p => p.Id == playPenaltySubmit.PlayId);
        if (play == null || (playPenaltySubmit.TeamId != play.Game.HomeTeamId && playPenaltySubmit.TeamId != play.Game.AwayTeamId))
        {
            return null;
        }

        Penalty? penalty = await _dbContext.Penalties.FindAsync(playPenaltySubmit.PenaltyId);
        if (penalty == null)
        {
            return null;
        }

        if (playPenaltySubmit.PlayerId != null)
        {
            Player? player = await _dbContext.Players.FindAsync(playPenaltySubmit.PlayerId);
            if (player == null || player.TeamId != playPenaltySubmit.TeamId)
            {
                return null;
            }
        }

        PlayPenalty newPlayPenalty = new()
        {
            PlayId = play.Id,
            PenaltyId = playPenaltySubmit.PenaltyId,
            PlayerId = playPenaltySubmit.PlayerId,
            TeamId = (int)playPenaltySubmit.TeamId,
            Enforced = playPenaltySubmit.Enforced,
            EnforcedFrom = playPenaltySubmit.EnforcedFrom,
            NoPlay = playPenaltySubmit.NoPlay,
            LossOfDown = playPenaltySubmit.LossOfDown,
            AutoFirstDown = playPenaltySubmit.AutoFirstDown,
            Yardage = playPenaltySubmit.Yardage ?? penalty.Yardage
        };

        _dbContext.PlayPenalties.Add(newPlayPenalty);
        await _dbContext.SaveChangesAsync();

        return newPlayPenalty;
    }

    public Task<bool> DeletePlayPenaltyAsync(int playPenaltyId)
    {
        throw new NotImplementedException();
    }

    public async Task<PlayPenalty?> GetSinglePlayPenaltyAsync(int playPenaltyId)
    {
        return await _dbContext.PlayPenalties.AsNoTracking().SingleOrDefaultAsync(pp => pp.Id == playPenaltyId);
    }

    public Task<PlayPenalty?> UpdatePlayPenaltyAsync(PlayPenaltySubmitDTO playPenaltySubmit)
    {
        throw new NotImplementedException();
    }
}