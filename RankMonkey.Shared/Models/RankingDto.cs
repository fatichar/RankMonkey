namespace RankMonkey.Shared.Models;

public record RankingDto(Guid UserId, float IncomePercentile, float NetWorthPercentile);