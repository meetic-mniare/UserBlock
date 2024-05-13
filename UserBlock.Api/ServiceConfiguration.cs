using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using UserBlock.Application;

namespace UserBlock.Api;

public static class ServiceConfiguration
{
    public static IServiceCollection AddConfigureAuthentication(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var jwtOptionsSection = configuration.GetRequiredSection(AppSettingConstants.JwtSetting);
        services.Configure<JwtSettings>(jwtOptionsSection);
        var jwtSettings = jwtOptionsSection.Get<JwtSettings>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key!))
                };
            });
        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().Build());
        return services;
    }
}