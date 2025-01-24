using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities;

public interface IConversionRepository
{
    Task<Conversion?> GetSingleConversionAsync(int conversionId);
    Task<Conversion?> CreateConversionAsync(PlaySubmitDTO playSubmit);
    Task<Conversion?> CreateConversionAsync(Conversion newConversion);
    Task<Conversion?> UpdateConversionAsync(Conversion conversionUpdate);
    Task<string?> DeleteConversionAsync(int conversionId);
}