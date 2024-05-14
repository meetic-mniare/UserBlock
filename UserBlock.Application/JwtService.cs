using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserBlock.Application.Interfaces;
using UserBlock.Contracts;

namespace UserBlock.Application;

public sealed class JwtService(IOptions<JwtSettings> jwtSettings) : IJwtService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));

    public string GenerateJwtToken(UserDto? user = null)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            GetClaims(user!.Id.ToString()),
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMin.GetValueOrDefault()),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static IEnumerable<Claim> GetClaims(string? userId)
    {
        if (!string.IsNullOrEmpty(userId))
            yield return new Claim(JwtRegisteredClaimNames.Sub, userId);
    }
}