using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;

    public TeamService(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<Team> CreateTeamAsync(TeamSubmitDTO teamSubmit)
    {
        return await _teamRepository.CreateTeamAsync(teamSubmit);
    }

    public async Task<Team> DeleteTeamAsync(int teamId, int userId)
    {
        return await _teamRepository.DeleteTeamAsync(teamId, userId);
    }

    public async Task<Team> GetSingleTeamAsync(int teamId, int userId)
    {
        return await _teamRepository.GetSingleTeamAsync(teamId, userId);
    }

    public async Task<List<Team>> GetTeamsAsync(int userId)
    {
        return await _teamRepository.GetTeamsAsync(userId);
    }

    public async Task<Team> UpdateTeamAsync(TeamSubmitDTO teamSubmit)
    {
        return await _teamRepository.UpdateTeamAsync(teamSubmit);
    }
}