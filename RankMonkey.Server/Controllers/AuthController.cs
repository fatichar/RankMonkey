using System.Security.Claims;
using RankMonkey.Server.Data;
using RankMonkey.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using RankMonkey.Server.Services;
using AutoMapper;

namespace RankMonkey.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthController(AuthService authService, JwtService jwtService, IMapper mapper)
    {
        _authService = authService;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var redirectUrl = Url.Action("OnGoogleResponse");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        // JWT logout is handled client-side by removing the token
        return Ok();
    }

    [HttpGet("user")]
    public IActionResult GetUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        int id = int.Parse(userId);

        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        var userInfo = _authService.GetUser(id);
        return Ok(new { IsAuthenticated = true, User = userInfo, Claims = claims });
    }

    [HttpGet("GoogleResponse")]
    public async Task<IActionResult> OnGoogleResponse()
    {
        var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authResult.Succeeded)
            return BadRequest();

        var token = await _authService.OnAuthenticated(authResult);

        return Ok(new { Token = token});
    }
}