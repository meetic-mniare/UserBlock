using UserBlock.Application.Interfaces;
using UserBlock.Contracts;

namespace UserBlock.Infrastructure;

public sealed class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserDto?> GetUser(Guid? userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        return (await userRepository.GetUser(userId.Value)).ToUserDto();
    }

    public async Task<UserDto?> GetUser(string? username)
    {
        ArgumentNullException.ThrowIfNull(username);

        return (await userRepository.GetUser(username)).ToUserDto();
    }

    public async Task<UserDto?> BlockUser(Guid? userId, string? username)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(username);

        return (await userRepository.BlockUser(userId.Value, username)).ToUserDto();
    }

    public async Task<UserDto?> DeleteBlock(Guid? userId, string? blokedUsername)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(blokedUsername);

        return (await userRepository.DeleteBlock(userId.Value, blokedUsername)).ToUserDto();
    }

    public bool IsAuthenticated(string? password, string? passwordHash)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(passwordHash);

        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
