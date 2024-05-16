using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace UserBlock.Infrastructure.Middleware;

public class AcceptLanguageMiddleware(RequestDelegate next, IConfiguration configuration)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var defaultAcceptLanguage =
            configuration.GetValue<string>("defaultAcceptLanguage") ?? "en-US";
        var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
        if (string.IsNullOrEmpty(acceptLanguage))
        {
            acceptLanguage = defaultAcceptLanguage;
        }
        context.Items["Accept-Language"] = acceptLanguage;

        await next(context);
    }
}
