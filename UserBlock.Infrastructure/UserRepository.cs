using Microsoft.EntityFrameworkCore;
using UserBlock.Application.Interfaces;
using UserBlock.Contracts;

namespace UserBlock.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    public UserRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserDto?> GetUser(string username)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);
        return user.ToUserDto();
    }

    public async Task<IList<Guid>> GetBlockedUsers(string username)
    {
        var user = await GetUser(username);
        return user?.BlockedUsers ?? [];
    }

    public async Task<bool> BlockUser(string username, string blokedUsername)
    {
        try
        {
            var user = await GetUser(username);
            var blockedUser = await GetUser(blokedUsername);
            if (user == null || blockedUser == null)
            {
                return false;
            }
            user.BlockedUsers?.Add(blockedUser.Id);
            _dbContext.Users.Update(user.ToEntity()!);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteBlock(string username, string? blokedUsername)
    {
        var user = await GetUser(username);
        var blockedUser = await GetUser(blokedUsername!);
        if (user == null || blockedUser == null)
        {
            return false;
        }
        user.BlockedUsers?.Remove(blockedUser.Id);
        _dbContext.Users.Update(user.ToEntity()!);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}