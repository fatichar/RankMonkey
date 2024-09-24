using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankMonkey.Server.Services;
using RankMonkey.Shared.Models;
using System.Security.Claims;

namespace RankMonkey.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/metrics")]
public class MetricsController(MetricsService metricsService) : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateMetricsRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var metrics = await metricsService.UpdateAsync(userId, request);

        return Ok(metrics);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var metricsDto = await metricsService.GetAsync(userId);
        return Ok(metricsDto);
    }
}