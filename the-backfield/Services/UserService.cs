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
        return await _userRepository.GetUserDataAsync(uid);
    }

    public async Task<int> VerifySessionKeyAsync(string sessionKey)
    {
        return await _userRepository.VerifySessionKeyAsync(sessionKey);
    }
}