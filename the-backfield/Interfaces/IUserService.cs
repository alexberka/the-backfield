using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Check if a sessionKey is valid and obtain associated userId
    /// </summary>
    /// <param name="sessionKey"></param>
    /// <returns>userId as int (if sessionKey is valid)<br/>Otherwise, 0 (if sessionKey is invalid)</returns>
    Task<int> VerifySessionKeyAsync(string sessionKey);
    Task<User?> GetUserDataAsync(string uid);
    Task<User?> CreateUserAsync(UserSubmitDTO userSubmit);
}