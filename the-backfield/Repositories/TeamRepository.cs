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

    public Task<Team> CreateTeamAsync(TeamSubmitDTO teamSubmit)
    {
        throw new NotImplementedException();
    }

    public Task<Team> DeleteTeamAsync(int teamId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<Team> GetSingleTeamAsync(int teamId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Team>> GetTeamsAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<Team> UpdateTeamAsync(TeamSubmitDTO teamSubmit)
    {
        throw new NotImplementedException();
    }
}