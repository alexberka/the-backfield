using TheBackfield.DTOs;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Repositories.PlayEntities;

public class ConversionRepository : IConversionRepository
{
    public Task<Conversion?> CreateConversionAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteConversionAsync(int conversionId)
    {
        throw new NotImplementedException();
    }

    public Task<Conversion?> GetSingleConversionAsync(int conversionId)
    {
        throw new NotImplementedException();
    }

    public Task<Conversion?> UpdateConversionAsync(PlaySubmitDTO playSubmit)
    {
        throw new NotImplementedException();
    }
}