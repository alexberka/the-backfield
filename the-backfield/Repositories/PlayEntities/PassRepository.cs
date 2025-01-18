using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using the_backfield.Migrations;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class PassRepository : IPassRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public PassRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Pass?> CreatePassAsync(PlaySubmitDTO playSubmit)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .Include(p => p.Game)
            .SingleOrDefaultAsync(p => p.Id == playSubmit.Id);

        if (play == null)
        {
            return null;
        }

        Player? passer = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == playSubmit.PasserId);
        if (passer == null || passer.TeamId != play.TeamId)
        {
            return null;
        }

        if (playSubmit.ReceiverId != null)
        {
            Player? receiver = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == playSubmit.ReceiverId);
            if (receiver == null || receiver.TeamId != play.TeamId)
            {
                return null;
            }
        }

        Pass newPass = new()
        {
            PlayId = playSubmit.Id,
            PasserId = playSubmit.PasserId,
            ReceiverId = playSubmit.ReceiverId,
            Completion = playSubmit.Completion
        };

        _dbContext.Passes.Add(newPass);
        await _dbContext.SaveChangesAsync();

        return newPass;
    }
    public async Task<Pass?> CreatePassAsync(Pass newPass)
    {
        Play? play = await _dbContext.Plays
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == newPass.PlayId);

        if (play == null)
        {
            return null;
        }

        Player? passer = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newPass.PasserId);
        if (passer == null || passer.TeamId != play.TeamId)
        {
            return null;
        }

        if (newPass.ReceiverId != null)
        {
            Player? receiver = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == newPass.ReceiverId);
            if (receiver == null || receiver.TeamId != play.TeamId)
            {
                return null;
            }
        }

        _dbContext.Passes.Add(newPass);
        await _dbContext.SaveChangesAsync();

        return newPass;
    }

    public Task<bool> DeletePassAsync(int passId)
    {
        throw new NotImplementedException();
    }

    public async Task<Pass?> GetSinglePassAsync(int passId)
    {
        return await _dbContext.Passes.FindAsync(passId);
    }

    public Task<Pass?> UpdatePassAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}