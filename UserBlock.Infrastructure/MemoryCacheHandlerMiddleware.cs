using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using UserBlock.Application;
namespace UserBlock.Infrastructure;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Threading.Tasks;

public class MemoryCacheHandlerMiddleware(RequestDelegate next, IMemoryCache cache, IConfiguration configuration)
{
    private const string CacheKeyPrefix = "UserId_";

    public async Task Invoke(HttpContext context)
    {
        var cacheKey = CacheKeyPrefix + context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            if (context.Request.Headers.ContainsKey("X-Skip-Cache"))
            {
                await next(context);
                return;
            }
            if (!cache.TryGetValue(cacheKey, out string? cachedResponse))
            {
                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;
                    await next(context);
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    cachedResponse = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }

                var cacheTimeoutInMinute = configuration.GetValue<double>(AppSettingConstants.CacheTimeoutInMinutes);
                cache.Set(cacheKey, cachedResponse, TimeSpan.FromMinutes(cacheTimeoutInMinute));
            }
            else
            {
                context.Response.ContentType = "application/json"; // Set your content type here
                await context.Response.WriteAsync(cachedResponse!);
            }
        }
        else if (context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                 context.Request.Method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            cache.Remove(cacheKey);
        }
        else
        {
            await next(context);
        }
    }
}