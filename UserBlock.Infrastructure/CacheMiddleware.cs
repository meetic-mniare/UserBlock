using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace UserBlock.Infrastructure;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Threading.Tasks;

public class CacheMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;

    public CacheMiddleware(RequestDelegate next, IMemoryCache cache, IConfiguration configuration)
    {
        _next = next;
        _cache = cache;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            var cacheKey = context.Request.Path + "/" + context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!_cache.TryGetValue(cacheKey, out string cachedResponse))
            {
                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;
                    await _next(context);
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    cachedResponse = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }

                var cacheTimeoutInMinutes = _configuration.GetValue<int>("CacheTimeoutInMinutes");
                _cache.Set(cacheKey, cachedResponse, TimeSpan.FromMinutes(cacheTimeoutInMinutes));
            }
            else
            {
                context.Response.ContentType = "application/json"; // Set your content type here
                await context.Response.WriteAsync(cachedResponse);
            }
        }
        else if (context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                 context.Request.Method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            // Invalidate cache for the corresponding route
            _cache.Remove(context.Request.Path);
        }
        else
        {
            await _next(context);
        }
    }
}