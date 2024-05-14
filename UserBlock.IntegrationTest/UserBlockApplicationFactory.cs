using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace UserBlock.IntegrationTest;

internal class UserBlockApplicationFactory(string cacheExpirationTime) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var appSettings = new Dictionary<string, string>
        {
            {"CacheTimeoutInMinutes", cacheExpirationTime}
        };
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(appSettings!);
        });
        
        builder.UseEnvironment("Development");
    }
   
}