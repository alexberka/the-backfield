﻿using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace the_backfield.Interfaces.PlayEntities
{
    public interface ISafetyRepository
    {
        Task<Safety?> GetSingleSafetyAsync(int safetyId);
        Task<Safety?> CreateSafetyAsync(PlaySubmitDTO playSubmit);
        Task<Safety?> UpdateSafetyAsync(PlaySubmitDTO playSubmit);
        Task<bool> DeleteSafetyAsync(int safetyId);
    }
}
