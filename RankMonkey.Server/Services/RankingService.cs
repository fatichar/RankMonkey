using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RankMonkey.Server.Data;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public class RankingService(ApplicationDbContext context, IMapper mapper, ILogger<RankingService> logger)
{
    public async Task<RankingDto> GetAsync(Guid userId)
    {
        var user = await context.Metrics.FindAsync(userId);
        if (user == null)
        {
            throw new Exception();
        }
        var total = await context.Metrics.CountAsync();

        float incomeRank = await context.Metrics
            .Where(m => m.Income > user.Income)
            .CountAsync();

        var incomePercentile = incomeRank / total;

        float netWorthRank = await context.Metrics
            .Where(m => m.NetWorth > user.NetWorth)
            .CountAsync();

        var netWorthPercentile = netWorthRank / total;

        return new RankingDto(userId, incomePercentile, netWorthPercentile);
    }
}