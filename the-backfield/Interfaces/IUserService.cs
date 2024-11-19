using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IUserService
{
    Task<int> VerifySessionKeyAsync(string sessionKey);
    Task<User> GetUserDataAsync(string uid);
    Task<User> CreateUserAsync(UserSubmitDTO userSubmit);
}