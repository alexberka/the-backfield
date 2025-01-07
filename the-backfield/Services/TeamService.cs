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

    public async Task<ResponseDTO<Team>> CreateTeamAsync(TeamSubmitDTO teamSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(teamSubmit.SessionKey);
        if (user == null)
        {
            return new ResponseDTO<Team> { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }
        Team newTeam = await _teamRepository.CreateTeamAsync(teamSubmit, user.Id);
        return new ResponseDTO<Team> { Resource = newTeam };
    }

    public async Task<ResponseDTO<Team>> DeleteTeamAsync(int teamId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Team? team = await _teamRepository.GetSingleTeamAsync(teamId);
        ResponseDTO<Team> teamCheck = SessionKeyClient.VerifyAccess(sessionKey, user, team);
        if (teamCheck.Error)
        {
            return teamCheck;
        }

        return new ResponseDTO<Team> { ErrorMessage = await _teamRepository.DeleteTeamAsync(teamId) };
    }

    public async Task<ResponseDTO<Team>> GetSingleTeamAsync(int teamId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Team? team = await _teamRepository.GetSingleTeamAsync(teamId);
        return SessionKeyClient.VerifyAccess(sessionKey, user, team);
    }

    public async Task<ResponseDTO<List<Team>>> GetTeamsBySessionKeyAsync(string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        if (user == null)
        {
            return new ResponseDTO<List<Team>> { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }
        List<Team> teams = await _teamRepository.GetTeamsByUserIdAsync(user.Id);
        return new ResponseDTO<List<Team>> { Resource = teams };
    }

    public async Task<ResponseDTO<Team>> UpdateTeamAsync(TeamSubmitDTO teamSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(teamSubmit.SessionKey);
        Team? team = await _teamRepository.GetSingleTeamAsync(teamSubmit.Id);
        ResponseDTO<Team> authCheck = SessionKeyClient.VerifyAccess(teamSubmit.SessionKey, user, team);
        if (authCheck.ErrorMessage != null)
        {
            return authCheck;
        }
        authCheck.Resource = await _teamRepository.UpdateTeamAsync(teamSubmit);
        return authCheck;
    }
}