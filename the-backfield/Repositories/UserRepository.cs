using Microsoft.EntityFrameworkCore;
using TheBackfield.Utilities;
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

    public async Task<User?> CreateUserAsync(UserSubmitDTO userSubmit)
    {
        bool userConflict = await _dbContext.Users.AnyAsync(u => u.Uid == userSubmit.Uid);

        if (userConflict)
        {
            return null;
        }

        User newUser = new()
        {
            Username = userSubmit.Username,
            CreatedOn = DateTime.Now,
            Uid = userSubmit.Uid
        };
        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        newUser.SessionKey = SessionKeyClient.Generate(newUser.Id);
        newUser.SessionStart = DateTime.Now;
        await _dbContext.SaveChangesAsync();

        return newUser;
    }

    public async Task<User?> GetUserDataAsync(string uid)
    {
        User? user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Uid == uid);

        if (user == null)
        {
            return user;
        }

        user.SessionKey = SessionKeyClient.Generate(user.Id);
        user.SessionStart = DateTime.Now;
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public Task<int> VerifySessionKeyAsync(string sessionKey)
    {
        throw new NotImplementedException();
    }
}