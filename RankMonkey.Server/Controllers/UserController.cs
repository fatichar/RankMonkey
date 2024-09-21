using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankMonkey.Server.Services;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(UserService userService, ILogger<UserController> logger) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var userInfo = userService.GetById(Guid.Parse(userId));
        return Ok(userInfo);
    }

    [Authorize(Roles = RoleNames.ADMIN_ROLE_NAME)]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var user = await userService.GetById(userId);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [Authorize(Roles = RoleNames.ADMIN_ROLE_NAME)]
    [HttpPut("{userId:guid}/role")]
    public async Task<IActionResult> UpdateUserRole(Guid userId, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            var updatedUser = await userService.UpdateUserRole(userId, request.NewRole);
            if (updatedUser == null)
                return NotFound("User not found");

            return Ok(updatedUser);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the user role.");
            return StatusCode(500, $"An error occurred while updating the user role: {ex.Message}");
        }
    }
}