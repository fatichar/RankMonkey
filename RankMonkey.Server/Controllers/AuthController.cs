using System.Security.Claims;
using RankMonkey.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IEnumerable<IAuthService> authServices) : ControllerBase
{
    [HttpPost("login/google")]
    public async Task<IActionResult> LoginWithGoogleAsync([FromBody] GoogleLoginRequest request)
    {
        var authService = authServices.FirstOrDefault(x => x.Name == AuthType.GOOGLE);
        if (authService == null)
            return BadRequest("Google login is not supported");
        try
        {
            var loginResult = await authService.LoginAsync(request.IdToken);
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
    public async Task<IActionResult> LoginAsGuestAsync()
    {
        var authService = authServices.FirstOrDefault(x => x.Name == AuthType.GUEST);
        if (authService == null)
            return BadRequest("Guest login is not supported");

        var loginResult = await authService.LoginAsync(string.Empty);
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