using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserBlock.Contracts;

namespace UserBlock.Api.Controllers;

 [Authorize]
[ApiController]
[Route("api/userBlock")]
public class UserBlockController: UserBlockControllerBase
{
    [HttpPost("block")]
    public IActionResult BlockUser(UserInfo userInfo)
    {
        return Ok( new
        {
            UserInfo = userInfo, 
            Blocked = true,
            BlockedReason = "User Blocked",
            BlockedDate = DateTime.Now,
            blockedBy = CurrentUserId,
        });
    }
    
}