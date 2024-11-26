using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface ITeamService
{
    Task<TeamResponseDTO> GetTeamsBySessionKeyAsync(string sessionKey);
    Task<TeamResponseDTO> GetSingleTeamAsync(int teamId, string sessionKey);
    Task<TeamResponseDTO> CreateTeamAsync(TeamSubmitDTO teamSubmit);
    Task<TeamResponseDTO> UpdateTeamAsync(TeamSubmitDTO teamSubmit);
    Task<TeamResponseDTO> DeleteTeamAsync(int teamId, string sessionKey);
}