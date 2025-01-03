using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface ITeamRepository
{
    Task<List<Team>> GetTeamsByUserIdAsync(int userId);
    Task<Team?> GetSingleTeamAsync(int teamId);
    Task<Team> CreateTeamAsync(TeamSubmitDTO teamSubmit, int userId);
    Task<Team?> UpdateTeamAsync(TeamSubmitDTO teamSubmit);
    Task<string?> DeleteTeamAsync(int teamId);
}