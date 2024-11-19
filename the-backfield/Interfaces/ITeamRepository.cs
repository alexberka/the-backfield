using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface ITeamRepository
{
    Task<List<Team>> GetTeamsAsync(int userId);
    Task<Team> GetSingleTeamAsync(int teamId, int userId);
    Task<Team> CreateTeamAsync(TeamSubmitDTO teamSubmit);
    Task<Team> UpdateTeamAsync(TeamSubmitDTO teamSubmit);
    Task<Team> DeleteTeamAsync(int teamId, int userId);
}