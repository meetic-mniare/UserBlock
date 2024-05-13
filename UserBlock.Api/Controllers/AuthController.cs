using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserBlock.Application.Interfaces;
using UserBlock.Contracts;

namespace UserBlock.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/authentication")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public AuthController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }
    
    [HttpPost]
    [Route("token")]
    public async Task<IActionResult> Token([FromBody] UserInfo user)
    {
        var storedUser = await _userService.GetUser(user?.Username);

        if (storedUser == null)
        {
            return Unauthorized("Unable to retrieve  user with username " + user?.Username);
        }
        
        if (!_userService.IsAuthenticated(user?.Password, storedUser.PasswordHash))
        {
            return Unauthorized();
        }
        
        var tokenString = _jwtService.GenerateJwtToken(storedUser);
        return Ok(new  Token{ Value = tokenString });
    }
}