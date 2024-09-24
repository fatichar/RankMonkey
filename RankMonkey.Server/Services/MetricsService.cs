using AutoMapper;
using RankMonkey.Server.Data;
using RankMonkey.Server.Entities;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public class MetricsService(ApplicationDbContext context, IMapper mapper, ILogger<MetricsService> logger)
{
    public async Task<MetricsDto> UpdateAsync(Guid userId, UpdateMetricsRequest request)
    {
        var metrics = await context.Metrics.FindAsync(userId);

        if (metrics != null)
        {
            return Update(metrics, request);
        }
        return Add(userId, request);
    }

    private MetricsDto Update(Metrics metrics, UpdateMetricsRequest request)
    {
        metrics.Currency = request.Currency;
        metrics.Income = request.Income;
        metrics.NetWorth = request.NetWorth;
        metrics.Timestamp = DateTime.UtcNow;

        context.Metrics.Update(metrics);
        context.SaveChanges();

        return mapper.Map<MetricsDto>(metrics);
    }

    private MetricsDto Add(Guid userId, UpdateMetricsRequest request)
    {
        var metrics = mapper.Map<Metrics>(request);
        metrics.UserId = userId;
        context.Metrics.Add(metrics);
        context.SaveChanges();

        return mapper.Map<MetricsDto>(metrics);
    }

    public async Task<MetricsDto?> GetAsync(Guid userId)
    {
        var metrics = await context.Metrics.FindAsync(userId);

        return metrics != null ? mapper.Map<MetricsDto>(metrics) : null;
    }
}