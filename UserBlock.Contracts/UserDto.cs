namespace UserBlock.Contracts;

public record UserDto(Guid Id, string Username, string PasswordHash, string Email, string Role);