using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserBlock.Api.Controllers.Abstraction;
using UserBlock.Application.Interfaces;
using UserBlock.Contracts;

namespace UserBlock.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/userBlock")]
public class UserBlockController : UserBlockControllerBase
{
    private readonly IUserService _userService;

    public UserBlockController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("GetUser")]
    public async Task<IActionResult> GetUser()
    {
        var user = await _userService.GetUser(CurrentUserId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [Route("BlockUser")]
    public async Task<IActionResult> BlockUser([FromBody] UserInfo user)
    {
        var result = await _userService.BlockUser(CurrentUserId, user.Username!);

        return result != null ? Ok(result) : new BadRequestResult();
    }

    [HttpDelete]
    [Route("UnblockUser")]
    public async Task<IActionResult> UnblockUser([FromBody] UserInfo user)
    {
        var result = await _userService.DeleteBlock(CurrentUserId, user.Username!);

        return result != null ? Ok(result) : new BadRequestResult();
    }
}