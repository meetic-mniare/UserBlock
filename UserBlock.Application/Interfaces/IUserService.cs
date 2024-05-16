using UserBlock.Contracts;

namespace UserBlock.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUser(Guid? username);
    Task<UserDto?> GetUser(string? username);
    bool IsAuthenticated(string? password, string? passwordHash);
    Task<UserDto?> BlockUser(Guid? userId, string? username);

    Task<UserDto?> DeleteBlock(Guid? userId, string? blokedUsername);
}
