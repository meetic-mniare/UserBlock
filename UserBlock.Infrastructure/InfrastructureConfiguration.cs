using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserBlock.Application.Interfaces;

namespace UserBlock.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<Random>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddHttpClient<ILocalizationApiClient, LocalizationApiClient>(client =>
        {
            var baseurl = configuration.GetValue<string>("LocalizationApi:BaseUrl");
            ArgumentNullException.ThrowIfNull(baseurl);
            client.BaseAddress = new Uri(baseurl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName: "UserDb");
        });
        services.AddMemoryCache();
        return services;
    }
}
