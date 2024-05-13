using UserBlock.Contracts;

namespace UserBlock.Application.Interfaces;

public interface IUserService
{
    UserDto? GetUser(string? username);
    bool IsAuthenticated(string? password, string? passwordHash);
}