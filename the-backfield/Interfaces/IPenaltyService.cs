using TheBackfield.DTOs;

namespace TheBackfield.Interfaces
{
    public interface IPenaltyService
    {
        Task<PenaltyResponseDTO> GetAllPenaltiesAsync(string sessionKey);
        Task<PenaltyResponseDTO> GetSinglePenaltyAsync(int penaltyId, string sessionKey);
        Task<PenaltyResponseDTO> CreatePenaltyAsync(PenaltySubmitDTO penaltySubmit);
        Task<PenaltyResponseDTO> UpdatePenaltyAsync(PenaltySubmitDTO penaltySubmit);
        Task<PenaltyResponseDTO> DeletePenaltyAsync(int penaltyId, string sessionKey);
    }
}
