using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace UserBlock.Api.Controllers.Abstraction;

public class UserBlockControllerBase : ControllerBase
{
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
