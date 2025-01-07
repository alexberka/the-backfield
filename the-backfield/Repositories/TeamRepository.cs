using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public TeamRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Team> CreateTeamAsync(TeamSubmitDTO teamSubmit, int userId)
    {
        Team newTeam = new()
        {
            LocationName = teamSubmit.LocationName ?? "",
            Nickname = teamSubmit.Nickname ?? "",
            HomeField = teamSubmit.HomeField ?? "",
            HomeLocation = teamSubmit.HomeLocation ?? "",
            LogoUrl = teamSubmit.LogoUrl ?? "",
            ColorPrimaryHex = teamSubmit.ColorPrimaryHex.ToLower(),
            ColorSecondaryHex = teamSubmit.ColorSecondaryHex.ToLower(),
            UserId = userId
        };

        _dbContext.Teams.Add(newTeam);
        await _dbContext.SaveChangesAsync();
        return newTeam;
    }

    public async Task<string?> DeleteTeamAsync(int teamId)
    {
        Team? team = await _dbContext.Teams.FindAsync(teamId);
        if (team == null)
        {
            return "Invalid team id";
        }

        _dbContext.Teams.Remove(team);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<Team?> GetSingleTeamAsync(int teamId)
    {
        return await _dbContext.Teams
            .Include(t => t.Players)
                .ThenInclude(p => p.Positions)
            .SingleOrDefaultAsync(t => t.Id == teamId);
    }

    public async Task<List<Team>> GetTeamsByUserIdAsync(int userId)
    {
        return await _dbContext.Teams.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<Team?> UpdateTeamAsync(TeamSubmitDTO teamSubmit)
    {
        Team? updateTeam = await _dbContext.Teams.SingleOrDefaultAsync(t => t.Id == teamSubmit.Id);
        
        if (updateTeam == null)
        {
            return null;
        }

        updateTeam.LocationName = teamSubmit.LocationName ?? updateTeam.LocationName;
        updateTeam.Nickname = teamSubmit.Nickname ?? updateTeam.Nickname;
        updateTeam.HomeField = teamSubmit.HomeField ?? updateTeam.HomeField;
        updateTeam.HomeLocation = teamSubmit.HomeLocation ?? updateTeam.HomeLocation;
        updateTeam.LogoUrl = teamSubmit.LogoUrl ?? updateTeam.LogoUrl;
        updateTeam.ColorPrimaryHex = teamSubmit.ColorPrimaryHex.ToLower();
        updateTeam.ColorSecondaryHex = teamSubmit.ColorSecondaryHex.ToLower();

        await _dbContext.SaveChangesAsync();
        return updateTeam;
    }
}