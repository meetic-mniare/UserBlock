namespace UserBlock.Contracts;

public record UserDto(Guid Id, string Username, string PasswordHash, string Email,IList<Guid>? BlockedUsers = null);