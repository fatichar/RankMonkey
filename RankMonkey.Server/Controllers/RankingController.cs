using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankMonkey.Server.Services;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Controllers;

[ApiController]
[Route("api/ranking")]
[Authorize]
public class RankingController(RankingService rankingService, ILogger<RankingController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<RankingDto>> GetRanking()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var ranking = await rankingService.GetAsync(userId);
            return Ok(ranking);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving ranking for user");
            return StatusCode(500, "An error occurred while retrieving the ranking.");
        }
    }
}