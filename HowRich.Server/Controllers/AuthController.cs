using System.Security.Claims;
using HowRich.Server.Data;
using HowRich.Server.Services;
using HowRich.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace HowRich.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpGet("login")]
    public IActionResult Login()
    {
        var redirectUrl = Url.Action("GoogleResponse");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    [HttpGet("user")]
    public IActionResult GetUser()
    {
        if (User.Identity.IsAuthenticated)
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(new { IsAuthenticated = true, Claims = claims });
        }
        return Ok(new { IsAuthenticated = false });
    }

    [HttpGet("GoogleResponse")]
    public async Task<IActionResult> GoogleResponse()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
            return BadRequest();

        var user = await authService.OnAuthenticated(authenticateResult);

        return Ok(user);
    }
}