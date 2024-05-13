using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserBlock.Application.Interfaces;

namespace UserBlock.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddDbContext<UserDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: "UserDb");
            }
        );
        services.AddMemoryCache();
        return services;
    }
}