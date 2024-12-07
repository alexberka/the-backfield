using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IConversionRepository
{
    Task<Conversion?> GetSingleConversionAsync(int conversionId);
    Task<Conversion?> CreateConversionAsync(PlaySubmitDTO playSubmit, int userId);
    Task<Conversion?> UpdateConversionAsync(PlaySubmitDTO playSubmit);
    Task<bool> DeleteConversionAsync(int conversionId);
}