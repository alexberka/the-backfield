using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces
{
    public interface IPenaltyRepository
    {
        Task<List<Penalty>> GetAllPenaltiesAsync(int userId);
        Task<Penalty?> GetSinglePenaltyAsync(int penaltyId);
        Task<Penalty?> CreatePenaltyAsync(PenaltySubmitDTO penaltySubmit, int userId);
        Task<Penalty?> UpdatePenaltyAsync(PenaltySubmitDTO penaltySubmit);
        Task<string?> DeletePenaltyAsync(int penaltyId);
    }
}
