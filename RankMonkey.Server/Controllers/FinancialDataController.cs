using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankMonkey.Server.Services;
using RankMonkey.Shared.Models;
using System.Security.Claims;

namespace RankMonkey.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FinancialDataController(FinancialDataService financialDataService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddOrUpdateFinancialData([FromBody] AddFinancialDataRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await financialDataService.AddOrUpdateFinancialDataAsync(userId, request.DataType, request.Value);

        if (result.IsSuccess)
        {
            return Ok(result.Payload);
        }

        return BadRequest(result.Message);
    }

    [HttpGet("ranking/{dataType}")]
    public async Task<IActionResult> GetRanking(string dataType, [FromQuery] decimal value)
    {
        var result = await financialDataService.GetRankingAsync(dataType, value);

        if (result.IsSuccess)
        {
            return Ok(new { Rank = result.Payload });
        }

        return BadRequest(result.Message);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserFinancialData()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var dataResult = await financialDataService.GetUserFinancialDataAsync(userId);
        var currencyResult = await financialDataService.GetUserCurrencyAsync(userId);

        if (dataResult.IsSuccess && currencyResult.IsSuccess)
        {
            return Ok(new { Data = dataResult.Payload, Currency = currencyResult.Payload });
        }

        return BadRequest(dataResult.Message ?? currencyResult.Message);
    }

    [HttpGet("currency")]
    public async Task<IActionResult> GetUserCurrency()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await financialDataService.GetUserCurrencyAsync(userId);

        if (result.IsSuccess)
        {
            return Ok(new { Currency = result.Payload });
        }

        return BadRequest(result.Message);
    }
}