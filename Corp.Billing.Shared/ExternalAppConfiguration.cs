using Microsoft.Extensions.DependencyInjection;

namespace Corp.Billing.Shared;

public static class ExternalAppConfiguration
{
    public static IServiceCollection AddExternalServices(this IServiceCollection services)
    {
        services.AddTransient<IBillingClientApi, BillingClientApi>();
        return services;
    }
}