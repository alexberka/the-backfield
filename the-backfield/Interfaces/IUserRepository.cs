using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserBySessionKeyAsync(string sessionKey);
    Task<User?> GetUserByUidAsync(string uid);
    Task<User?> CreateUserAsync(UserSubmitDTO userSubmit);
}