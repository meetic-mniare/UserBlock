using UserBlock.Contracts;

namespace UserBlock.Application.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(UserDto? user = null);
}
