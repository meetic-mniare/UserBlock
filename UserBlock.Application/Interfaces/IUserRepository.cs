using UserBlock.Contracts;
using UserBlock.Domain;

namespace UserBlock.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUser(Guid userId);

    Task<User?> BlockUser(Guid userId, string blokedUsername);

    Task<User?> DeleteBlock(Guid userId, string blockedUSername);
    Task<User?> GetUser(string userId);
}