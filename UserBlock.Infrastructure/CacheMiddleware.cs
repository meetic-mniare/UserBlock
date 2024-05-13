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

    public CacheMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            var cacheKey = context.Request.Path;
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
                _cache.Set(cacheKey, cachedResponse, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
            }
            else
            {
                context.Response.ContentType = "application/json"; // Set your content type here
                await context.Response.WriteAsync(cachedResponse);
            }
        }
        else if (context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) || context.Request.Method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
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
