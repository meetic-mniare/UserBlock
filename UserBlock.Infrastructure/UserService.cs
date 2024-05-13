using UserBlock.Application.Interfaces;
using UserBlock.Contracts;

namespace UserBlock.Infrastructure;

public sealed class UserService(IUserRepository userRepository) : IUserService
{
    public Task<UserDto?> GetUser(Guid? userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        return userRepository.GetUser(userId.Value);
    }

    public Task<UserDto?> GetUser(string? username)
    {
        ArgumentNullException.ThrowIfNull(username);

        return userRepository.GetUser(username);
    }
    
    public Task<UserDto?> BlockUser(Guid? userId, string? username)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(username);

        return userRepository.BlockUser(userId.Value, username);
    }

    public Task<UserDto?> DeleteBlock(Guid? userId, string? blokedUsername)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(blokedUsername);

        return userRepository.DeleteBlock(userId.Value, blokedUsername);
    }
    

    public bool IsAuthenticated(string? password, string? passwordHash)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(passwordHash);

        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}