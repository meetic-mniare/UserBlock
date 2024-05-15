using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UserBlock.Application.Interfaces;

namespace UserBlock.Api.Controllers.Abstraction;

public class UserBlockControllerBase(ILocalizationApiClient localizationApiClient ) : ControllerBase
{
    protected virtual async Task<IActionResult> GetExceptionMessage(Exception ex)
    {
        var initialMessage = ex.Message;
        var localizedMessage =
            await localizationApiClient.TranslateAsync(initialMessage, CultureInfo.CurrentCulture.Name);
        StatusCode(500, new
        {
            Message = localizedMessage,
            Exception = ex
        });
        return StatusCode(500, new
        {
            Message = localizedMessage,
            Exception = ex
        });
    }
    protected Guid CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId)
                ? userId
                : throw new ArgumentNullException("userId");
        }
    }
    
}