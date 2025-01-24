using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class KickoffRepository : IKickoffRepository
{
    private readonly TheBackfieldDbContext _dbContext;
    public KickoffRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Kickoff?> CreateKickoffAsync(PlaySubmitDTO playSubmit)
    {
        Play? play = await _dbContext.Plays.FindAsync(playSubmit.Id);
        if (play == null)
        {
            return null;
        }

        if (playSubmit.KickerId != null)
        {
            Player? kicker = await _dbContext.Players.FindAsync(playSubmit.KickerId);
            if (kicker == null)
            {
                return null;
            }
        }

        if (playSubmit.KickReturnerId != null)
        {
            Player? returner = await _dbContext.Players.FindAsync(playSubmit.KickReturnerId);
            if (returner == null)
            {
                return null;
            }
        }

        Kickoff newKickoff = new()
        {
            PlayId = playSubmit.Id,
            KickerId = playSubmit.KickerId,
            ReturnerId = playSubmit.KickReturnerId,
            FieldedAt = playSubmit.KickFieldedAt,
            Touchback = playSubmit.KickTouchback
        };

        _dbContext.Kickoffs.Add(newKickoff);
        await _dbContext.SaveChangesAsync();

        return newKickoff;
    }

    public async Task<Kickoff?> CreateKickoffAsync(Kickoff newKickoff)
    {
        Play? play = await _dbContext.Plays.FindAsync(newKickoff.PlayId);
        if (play == null)
        {
            return null;
        }

        if (newKickoff.KickerId != null)
        {
            Player? kicker = await _dbContext.Players.FindAsync(newKickoff.KickerId);
            if (kicker == null)
            {
                return null;
            }
        }

        if (newKickoff.ReturnerId != null)
        {
            Player? returner = await _dbContext.Players.FindAsync(newKickoff.ReturnerId);
            if (returner == null)
            {
                return null;
            }
        }

        _dbContext.Kickoffs.Add(newKickoff);
        await _dbContext.SaveChangesAsync();

        return newKickoff;
    }

    public async Task<Kickoff?> UpdateKickoffAsync(Kickoff kickoffUpdate)
    {
        Kickoff? kickoff = await _dbContext.Kickoffs.FindAsync(kickoffUpdate.Id);
        if (kickoff == null)
        {
            return null;
        }

        if (kickoffUpdate.KickerId != null)
        {
            Player? kicker = await _dbContext.Players.FindAsync(kickoffUpdate.KickerId);
            if (kicker == null)
            {
                return null;
            }
        }

        if (kickoffUpdate.ReturnerId != null)
        {
            Player? returner = await _dbContext.Players.FindAsync(kickoffUpdate.ReturnerId);
            if (returner == null)
            {
                return null;
            }
        }

        kickoff.KickerId = kickoffUpdate.KickerId;
        kickoff.TeamId = kickoffUpdate.TeamId;
        kickoff.ReturnerId = kickoffUpdate.ReturnerId;
        kickoff.ReturnTeamId = kickoffUpdate.ReturnTeamId;
        kickoff.FieldedAt = kickoffUpdate.FieldedAt;
        kickoff.Touchback = kickoffUpdate.Touchback;
        kickoff.Distance = kickoffUpdate.Distance;
        kickoff.ReturnYardage = kickoffUpdate.ReturnYardage;

        await _dbContext.SaveChangesAsync();

        return kickoff;
    }

    public async Task<string?> DeleteKickoffAsync(int kickoffId)
    {
        Kickoff? kickoff = await _dbContext.Kickoffs.FindAsync(kickoffId);
        if (kickoff == null)
        {
            return "Invalid kickoff id";
        }

        _dbContext.Kickoffs.Remove(kickoff);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<Kickoff?> GetSingleKickoffAsync(int kickoffId)
    {
        return await _dbContext.Kickoffs.AsNoTracking().SingleOrDefaultAsync(k => k.Id == kickoffId);
    }
}