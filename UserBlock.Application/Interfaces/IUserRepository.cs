using UserBlock.Contracts;

namespace UserBlock.Application.Interfaces;

public interface IUserRepository
{
    Task<UserDto?> GetUser(Guid userId);

    Task<UserDto?> BlockUser(Guid userId, string blokedUsername);

    Task<UserDto?> DeleteBlock(Guid userId, string blockedUSername);
    Task<UserDto?> GetUser(string userId);
}