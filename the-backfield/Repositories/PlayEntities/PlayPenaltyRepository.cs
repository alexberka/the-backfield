using System.ComponentModel.DataAnnotations;
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

    public async Task<PlayPenalty?> CreatePlayPenaltyAsync(PlayPenalty newPlayPenalty)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .Include(p => p.Game)
            .SingleOrDefaultAsync(p => p.Id == newPlayPenalty.PlayId);
        if (play == null || play.Game == null || !new int[] {play.Game.HomeTeamId, play.Game.AwayTeamId}.Contains(newPlayPenalty.TeamId))
        {
            return null;
        }

        Penalty? penalty = await _dbContext.Penalties.FindAsync(newPlayPenalty.PenaltyId);
        if (penalty == null)
        {
            return null;
        }

        if (newPlayPenalty.PlayerId != null)
        {
            Player? player = await _dbContext.Players.FindAsync(newPlayPenalty.PlayerId);
            if (player == null || player.TeamId != newPlayPenalty.TeamId)
            {
                return null;
            }
        }

        _dbContext.PlayPenalties.Add(newPlayPenalty);
        await _dbContext.SaveChangesAsync();

        return newPlayPenalty;
    }
    public async Task<PlayPenalty?> UpdatePlayPenaltyAsync(PlayPenalty playPenaltyUpdate)
    {
        PlayPenalty? playPenalty = await _dbContext.PlayPenalties.SingleOrDefaultAsync(p => p.Id == playPenaltyUpdate.Id);
        if (playPenalty == null)
        {
            return null;
        }

        Penalty? penalty = await _dbContext.Penalties.FindAsync(playPenalty.PenaltyId);
        if (penalty == null)
        {
            return null;
        }

        if (playPenaltyUpdate.PlayerId != null)
        {
            Player? player = await _dbContext.Players.FindAsync(playPenaltyUpdate.PlayerId);
            if (player == null)
            {
                return null;
            }
        }

        playPenalty.PenaltyId = playPenaltyUpdate.PenaltyId;
        playPenalty.PlayerId = playPenaltyUpdate.PlayerId;
        playPenalty.TeamId = playPenaltyUpdate.TeamId;
        playPenalty.Enforced = playPenaltyUpdate.Enforced;
        playPenalty.EnforcedFrom = playPenaltyUpdate.EnforcedFrom;
        playPenalty.NoPlay = playPenaltyUpdate.NoPlay;
        playPenalty.LossOfDown = playPenaltyUpdate.LossOfDown;
        playPenalty.AutoFirstDown = playPenaltyUpdate.AutoFirstDown;
        playPenalty.Yardage = playPenaltyUpdate.Yardage;

        await _dbContext.SaveChangesAsync();

        return playPenalty;
    }

    public async Task<string?> DeletePlayPenaltyAsync(int playPenaltyId)
    {
        PlayPenalty? playPenalty = await _dbContext.PlayPenalties.FindAsync(playPenaltyId);
        if (playPenalty == null)
        {
            return "Invalid play penalty id";
        }

        _dbContext.PlayPenalties.Remove(playPenalty);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<PlayPenalty?> GetSinglePlayPenaltyAsync(int playPenaltyId)
    {
        return await _dbContext.PlayPenalties.AsNoTracking().SingleOrDefaultAsync(pp => pp.Id == playPenaltyId);
    }
}