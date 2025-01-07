using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces
{
    public interface IPenaltyService
    {
        Task<ResponseDTO<List<Penalty>>> GetAllPenaltiesAsync(string sessionKey);
        Task<ResponseDTO<Penalty>> GetSinglePenaltyAsync(int penaltyId, string sessionKey);
        Task<ResponseDTO<Penalty>> CreatePenaltyAsync(PenaltySubmitDTO penaltySubmit);
        Task<ResponseDTO<Penalty>> UpdatePenaltyAsync(PenaltySubmitDTO penaltySubmit);
        Task<ResponseDTO<Penalty>> DeletePenaltyAsync(int penaltyId, string sessionKey);
    }
}
