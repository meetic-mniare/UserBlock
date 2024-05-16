using System.Globalization;
using UserBlock.Application.Interfaces;

namespace UserBlock.Infrastructure.Middleware;

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger,
    ILocalizationApiClient localizationApiClient
)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        const HttpStatusCode code = HttpStatusCode.InternalServerError;
        var message = exception.Message;
        var translatedMessage = localizationApiClient
            .TranslateAsync(message, CultureInfo.CurrentCulture.Name)
            .GetAwaiter()
            .GetResult();
        var result = JsonConvert.SerializeObject(new { Message = translatedMessage });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}
