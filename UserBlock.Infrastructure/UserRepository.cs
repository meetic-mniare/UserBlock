using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserBlock.Application.Interfaces;
using UserBlock.Contracts;
using UserBlock.Domain;

namespace UserBlock.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    public UserRepository(UserDbContext dbContext)
    {
        var initialUsers = new List<User>
        {
            new User(Guid.NewGuid(), "user1", BCrypt.Net.BCrypt.HashPassword("password1"), "user1@gmail.com",
                []),
            new User(Guid.NewGuid(), "user2", BCrypt.Net.BCrypt.HashPassword("password2"), "user2@gmail.com",
                [])
        };
        dbContext.Users.AddRange(initialUsers);
        dbContext.SaveChanges();

        _dbContext = dbContext;
    }

    public async Task<UserDto?> GetUser(string username)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        return user.ToUserDto();
    }

    public async Task<UserDto?> GetUser(Guid userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return user.ToUserDto();
    }

    public async Task<UserDto?> BlockUser(Guid userId, string userName)
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

    public async Task<UserDto?> DeleteBlock(Guid userId, string blockedUSername)
    {
        var user = await GetUser(userId);
        var blockedUser = await GetUser(blockedUSername);
        if (user == null || blockedUser == null)
        {
            return null;
        }

        user.BlockedUsers?.Remove(blockedUser.Id);
        await _dbContext.SaveChangesAsync();
        return user;
    }
}