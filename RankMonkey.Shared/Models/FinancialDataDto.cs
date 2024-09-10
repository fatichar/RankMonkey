namespace RankMonkey.Shared.Models;

public class FinancialDataDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string DataType { get; set; } = null!;
    public long Value { get; set; }
    public DateTime Timestamp { get; set; }
}

public class AddFinancialDataRequest
{
    public string DataType { get; set; } = null!;
    public long Value { get; set; }
}