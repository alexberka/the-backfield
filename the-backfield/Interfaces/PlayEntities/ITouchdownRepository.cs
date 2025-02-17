﻿using TheBackfield.DTOs;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Interfaces.PlayEntities
{
    public interface ITouchdownRepository
    {
        Task<Touchdown?> GetSingleTouchdownAsync(int touchdownId);
        Task<Touchdown?> CreateTouchdownAsync(PlaySubmitDTO playSubmit);
        Task<Touchdown?> CreateTouchdownAsync(Touchdown newTouchdown);
        Task<Touchdown?> UpdateTouchdownAsync(Touchdown touchdownUpdate);
        Task<string?> DeleteTouchdownAsync(int touchdownId);
    }
}
