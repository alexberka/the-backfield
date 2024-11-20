using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> CreateUserAsync(UserSubmitDTO userSubmit)
    {
        return await _userRepository.CreateUserAsync(userSubmit);
    }

    public async Task<User?> GetUserDataAsync(string uid)
    {
        return await _userRepository.GetUserByUidAsync(uid);
    }
    /// <summary>
    /// Returns userId if sessionKey is valid, otherwise returns 0
    /// </summary>
    /// <param name="sessionKey"></param>
    /// <returns>userId as int</returns>
    public async Task<int> VerifySessionKeyAsync(string sessionKey)
    {
        User? verified = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        return verified == null ? 0 : verified.Id;
    }
}