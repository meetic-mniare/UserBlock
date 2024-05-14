using Microsoft.Extensions.DependencyInjection;
using UserBlock.Application.Interfaces;

namespace UserBlock.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddTransient<IJwtService, JwtService>();
        return services;
    }
}