namespace UserBlock.Domain;

public record User(Guid Id, string Username, string PasswordHash, string Email, IList<Guid>? BlockedUsers = null);