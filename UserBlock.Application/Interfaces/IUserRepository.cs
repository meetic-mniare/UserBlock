using UserBlock.Contracts;

namespace UserBlock.Application.Interfaces;

public interface IUserRepository
{
    Task<UserDto?> GetUser(string username);

    Task<IList<Guid>> GetBlockedUsers(string username);

    Task<bool> BlockUser(string username, string blokedUsername);

    Task<bool> DeleteBlock(string username, string blokedUsername);
}