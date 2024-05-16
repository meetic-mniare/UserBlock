using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserBlock.Api.Controllers.Abstraction;
using UserBlock.Application.Interfaces;
using UserBlock.Contracts;

namespace UserBlock.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/userBlock")]
// public class UserBlockController(IUserService userService, ILocalizationApiClient localizationApiClient)
public class UserBlockController(IUserService userService) : UserBlockControllerBase
{
    [HttpGet("GetUser")]
    public async Task<IActionResult> GetUser()
    {
        // var test = await localizationApiClient.TranslateAsync("Welcome_Message", "es-ES");
        var user = await userService.GetUser(CurrentUserId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new UserResponse(user, DateTime.UtcNow));
    }

    [HttpPost]
    [Authorize(Policy = "BillingClientApiPolicy")]
    [Route("BlockUser")]
    public async Task<IActionResult> BlockUser([FromBody] UserRequest user)
    {
        var result = await userService.BlockUser(CurrentUserId, user.Username!);

        return result != null
            ? Ok(new UserResponse(result, DateTime.UtcNow))
            : new BadRequestResult();
    }

    [HttpDelete]
    [Authorize(Policy = "BillingClientApiPolicy")]
    [Route("UnblockUser")]
    public async Task<IActionResult> UnblockUser([FromBody] UserRequest user)
    {
        var result = await userService.DeleteBlock(CurrentUserId, user.Username!);

        return result != null
            ? Ok(new UserResponse(result, DateTime.UtcNow))
            : new BadRequestResult();
    }
}
