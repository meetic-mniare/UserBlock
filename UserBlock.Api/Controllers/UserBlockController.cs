using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserBlock.Contracts;

namespace UserBlock.Api.Controllers;

 [Authorize]
[ApiController]
[Route("api/userBlock")]
public class UserBlockController: ControllerBase
{
    [HttpPost("block")]
    public IActionResult BlockUser(UserInfo userInfo)
    {
        return Ok(userInfo);
    }
    
}