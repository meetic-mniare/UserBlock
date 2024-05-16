using UserBlock.Contracts;
using UserBlock.Domain;

namespace UserBlock.Infrastructure;

public static class ModelExtensions
{
    public static UserDto? ToUserDto(this User? user)
    {
        return user == null
            ? null
            : new UserDto(user.Id, user.Username, user.PasswordHash, user.Email, user.BlockedUsers);
    }
}
