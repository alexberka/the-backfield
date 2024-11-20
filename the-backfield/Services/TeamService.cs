using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Utilities;

namespace TheBackfield.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;

    public TeamService(ITeamRepository teamRepository, IUserRepository userRepository)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
    }

    public async Task<ResponseDTO> CreateTeamAsync(TeamSubmitDTO teamSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(teamSubmit.SessionKey);
        if (user == null)
        {
            return new ResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }
        Team newTeam = await _teamRepository.CreateTeamAsync(teamSubmit, user.Id);
        return new ResponseDTO { Resource = newTeam, ResourceId = newTeam.Id };
    }

    public async Task<Team> DeleteTeamAsync(int teamId, int userId)
    {
        return await _teamRepository.DeleteTeamAsync(teamId);
    }

    public async Task<Team?> GetSingleTeamAsync(int teamId, int userId)
    {
        return await _teamRepository.GetSingleTeamAsync(teamId);
    }

    public async Task<List<Team>> GetTeamsAsync(int userId)
    {
        return await _teamRepository.GetTeamsAsync(userId);
    }

    public async Task<ResponseDTO> UpdateTeamAsync(TeamSubmitDTO teamSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(teamSubmit.SessionKey);
        Team? team = await _teamRepository.GetSingleTeamAsync(teamSubmit.Id);
        ResponseDTO authCheck = SessionKeyClient.VerifyAccess(teamSubmit.SessionKey, user, team);
        if (authCheck.ErrorMessage != null)
        {
            return authCheck;
        }
        authCheck.Resource = await _teamRepository.UpdateTeamAsync(teamSubmit);
        return authCheck;
    }
}