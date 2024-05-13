using System.ComponentModel.DataAnnotations;

namespace UserBlock.Application;

public class JwtSettings
{
    public string? Issuer { get; init; } = string.Empty;

    public string? Audience { get; init; } = string.Empty;
    public string? Key { get; init; } =  string.Empty;

    public string? SigningAlgorithm { get; init; } = string.Empty;
    public double? ExpirationInMin { get; init; } = 30;
}