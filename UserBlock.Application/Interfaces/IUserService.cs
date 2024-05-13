using UserBlock.Contracts;

namespace UserBlock.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUser(string? username);
    bool IsAuthenticated(string? password, string? passwordHash);
}