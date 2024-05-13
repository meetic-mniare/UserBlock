using UserBlock.Application.Interfaces;
using UserBlock.Contracts;

namespace UserBlock.Infrastructure;

public sealed class UserService(IUserRepository userRepository) : IUserService
{
    public Task<UserDto?> GetUser(string? username)
    {
        ArgumentNullException.ThrowIfNull(username);

        return userRepository.GetUser(username);
    }
    
    public Task<IList<Guid>> GetBlockedUsers(string? username)
    {
        ArgumentNullException.ThrowIfNull(username);

        return userRepository.GetBlockedUsers(username);
    }
    
    public Task<bool> BlockUser(string? username, string? blokedUsername)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(blokedUsername);

        return userRepository.BlockUser(username, blokedUsername);
    }

    public Task<bool> DeleteBlock(string? username, string? blokedUsername)
    {
        ArgumentNullException.ThrowIfNull(username);

        return userRepository.DeleteBlock(username, blokedUsername!);
    }
    public bool IsAuthenticated(string? password, string? passwordHash)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(passwordHash);

        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}