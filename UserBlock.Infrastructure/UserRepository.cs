using Microsoft.EntityFrameworkCore;
using UserBlock.Application.Interfaces;
using UserBlock.Domain;

namespace UserBlock.Infrastructure;

internal class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    internal UserRepository(UserDbContext dbContext)
    {
        var initialUsers = new List<User>
        {
            new(
                Guid.NewGuid(),
                "user1",
                BCrypt.Net.BCrypt.HashPassword("password1"),
                "user1@gmail.com",
                []
            ),
            new User(
                Guid.NewGuid(),
                "user2",
                BCrypt.Net.BCrypt.HashPassword("password2"),
                "user2@gmail.com",
                []
            )
        };
        dbContext.Users.AddRange(initialUsers);
        dbContext.SaveChanges();

        _dbContext = dbContext;
    }

    public async Task<User?> GetUser(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUser(Guid userId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> BlockUser(Guid userId, string userName)
    {
        var user = await GetUser(userId);
        var blockedUser = await GetUser(userName);
        if (user == null || blockedUser == null)
        {
            return null;
        }

        user.BlockedUsers?.Add(blockedUser.Id);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> DeleteBlock(Guid userId, string blockedUserName)
    {
        var user = await GetUser(userId);
        var blockedUser = await GetUser(blockedUserName);
        if (user == null || blockedUser == null)
        {
            return null;
        }

        user.BlockedUsers?.Remove(blockedUser.Id);
        await _dbContext.SaveChangesAsync();
        return user;
    }
}
