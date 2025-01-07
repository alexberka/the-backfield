using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface ITeamService
{
    Task<ResponseDTO<List<Team>>> GetTeamsBySessionKeyAsync(string sessionKey);
    Task<ResponseDTO<Team>> GetSingleTeamAsync(int teamId, string sessionKey);
    Task<ResponseDTO<Team>> CreateTeamAsync(TeamSubmitDTO teamSubmit);
    Task<ResponseDTO<Team>> UpdateTeamAsync(TeamSubmitDTO teamSubmit);
    Task<ResponseDTO<Team>> DeleteTeamAsync(int teamId, string sessionKey);
}