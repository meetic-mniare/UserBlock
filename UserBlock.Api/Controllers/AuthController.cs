using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserBlock.Application.Interfaces;
using UserBlock.Contracts;

namespace UserBlock.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/authentication")]
public class AuthController(IUserService userService, IJwtService jwtService) : ControllerBase
{
    [HttpPost]
    [Route("token")]
    public async Task<IActionResult> Token([FromBody] UserRequest user)
    {
        var storedUser = await userService.GetUser(user?.Username);

        if (storedUser == null)
        {
            return Unauthorized("Unable to retrieve  user with username " + user?.Username);
        }
        
        if (!userService.IsAuthenticated(user?.Password, storedUser.PasswordHash))
        {
            return Unauthorized();
        }
        
        var tokenString = jwtService.GenerateJwtToken(storedUser);
        return Ok(new  JwtToken{ Value = tokenString });
    }
}