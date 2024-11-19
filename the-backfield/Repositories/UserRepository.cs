using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public UserRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public Task<User> CreateUserAsync(UserSubmitDTO userSubmit)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserDataAsync(string uid)
    {
        throw new NotImplementedException();
    }

    public Task<int> VerifySessionKeyAsync(string sessionKey)
    {
        throw new NotImplementedException();
    }
}