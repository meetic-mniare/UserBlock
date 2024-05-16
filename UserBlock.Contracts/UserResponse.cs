namespace UserBlock.Contracts;

public record UserResponse(UserDto? Data, DateTimeOffset RetrieveTime);
