using System.Security.Claims;
using RankMonkey.Server.Services;
using RankMonkey.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace RankMonkey.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(AuthService authService, JwtService jwtService) : ControllerBase
{
    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] string idToken)
    {
        try
        {
            var user = await authService.VerifyGoogleToken(idToken);
            if (user == null)
                return Unauthorized();

            var jwtToken = jwtService.GenerateToken(user);
            return Ok(new { Token = jwtToken });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("user")]
    public IActionResult GetUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var userInfo = authService.GetUser(int.Parse(userId));
        return Ok(userInfo);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Server-side logout logic (if needed)
        return Ok();
    }
}