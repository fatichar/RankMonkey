namespace RankMonkey.Shared.Models;

public record MetricsDto(Guid UserId, long Income, long NetWorth, DateTime Timestamp, string Currency);

public record UpdateMetricsRequest(long Income, long NetWorth, string Currency);