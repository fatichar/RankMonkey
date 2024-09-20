using RankMonkey.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(AuthService authService, JwtService jwtService) : ControllerBase
{
    [HttpPost("login/google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var user = await authService.VerifyGoogleToken(request.IdToken);
            if (user == null)
                return Unauthorized();

            var jwtToken = jwtService.GenerateToken(user);
            return Ok(new TokenResponse(jwtToken));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok();
    }
}