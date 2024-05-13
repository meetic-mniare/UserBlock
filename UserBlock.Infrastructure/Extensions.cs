using UserBlock.Contracts;
using UserBlock.Domain;

namespace UserBlock.Infrastructure;

public static class Extensions
{
    public static UserDto? ToUserDto(this User? user)
    {
        return user == null
            ? null
            : new UserDto(user.Id, user.Username, user.PasswordHash, user.Email, user.BlockedUsers);
    }

    public static User? ToEntity(this UserDto? userDto)
    {
        return userDto == null
            ? null
            : new User(userDto.Id, userDto.Username, userDto.PasswordHash, userDto.Email,
                userDto.BlockedUsers);
    }
}