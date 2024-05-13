using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserBlock.Application.Interfaces;

namespace UserBlock.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services,
        ConfigurationManager configurationManager)
    {

        services.AddTransient<IJwtService, JwtService>();
        services.AddTransient<IUserService, UserService>();
        return services;
    }
}