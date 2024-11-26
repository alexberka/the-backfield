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

    public async Task<TeamResponseDTO> CreateTeamAsync(TeamSubmitDTO teamSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(teamSubmit.SessionKey);
        if (user == null)
        {
            return new TeamResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }
        Team newTeam = await _teamRepository.CreateTeamAsync(teamSubmit, user.Id);
        return new TeamResponseDTO { Team = newTeam };
    }

    public async Task<TeamResponseDTO> DeleteTeamAsync(int teamId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Team? team = await _teamRepository.GetSingleTeamAsync(teamId);
        TeamResponseDTO teamCheck = SessionKeyClient.VerifyAccess(sessionKey, user, team);
        if (teamCheck.Error)
        {
            return teamCheck;
        }

        return new TeamResponseDTO { ErrorMessage = await _teamRepository.DeleteTeamAsync(teamId) };
    }

    public async Task<TeamResponseDTO> GetSingleTeamAsync(int teamId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Team? team = await _teamRepository.GetSingleTeamAsync(teamId);
        return SessionKeyClient.VerifyAccess(sessionKey, user, team);
    }

    public async Task<TeamResponseDTO> GetTeamsBySessionKeyAsync(string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        if (user == null)
        {
            return new TeamResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }
        List<Team> teams = await _teamRepository.GetTeamsByUserIdAsync(user.Id);
        return new TeamResponseDTO { Teams = teams };
    }

    public async Task<TeamResponseDTO> UpdateTeamAsync(TeamSubmitDTO teamSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(teamSubmit.SessionKey);
        Team? team = await _teamRepository.GetSingleTeamAsync(teamSubmit.Id);
        TeamResponseDTO authCheck = SessionKeyClient.VerifyAccess(teamSubmit.SessionKey, user, team);
        if (authCheck.ErrorMessage != null)
        {
            return authCheck;
        }
        authCheck.Team = await _teamRepository.UpdateTeamAsync(teamSubmit);
        return authCheck;
    }
}