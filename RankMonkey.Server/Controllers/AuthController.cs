using RankMonkey.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(AuthenticationService authService) : ControllerBase
{
    [HttpPost("login/google")]
    public async Task<IActionResult> LoginWithGoogleAsync([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var loginResult = await authService.LoginAsync(AuthType.Google, request.IdToken);
            if (loginResult.IsFailure)
                return Unauthorized();

            return Ok(loginResult.Payload);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login/guest")]
    public IActionResult LoginAsGuest()
    {
        var loginResult = authService.LoginAsGuest();
        if (loginResult.IsFailure)
            return Unauthorized();

        return Ok(loginResult.Payload);
    }

    [Authorize]
    [HttpPost("logout")]
    public Task<IActionResult> Logout()
    {
        return Task.FromResult<IActionResult>(Ok());
    }
}