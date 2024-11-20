using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface ITeamService
{
    Task<List<Team>> GetTeamsAsync(int userId);
    Task<Team?> GetSingleTeamAsync(int teamId, int userId);
    Task<ResponseDTO> CreateTeamAsync(TeamSubmitDTO teamSubmit);
    Task<ResponseDTO> UpdateTeamAsync(TeamSubmitDTO teamSubmit);
    Task<Team> DeleteTeamAsync(int teamId, int userId);
}