using Corp.Billing.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UserBlock.IntegrationTest;

internal class UserBlockApplicationFactory(string cacheExpirationTime) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services => { services.AddTransient<IBillingClientApi, MockedBillingClientApi>(); });
        var appSettings = new Dictionary<string, string>
        {
            { "CacheTimeoutInMinutes", cacheExpirationTime }
        };
        builder.ConfigureAppConfiguration((context, config) => { config.AddInMemoryCollection(appSettings!); });

        builder.UseEnvironment("Development");
    }
}