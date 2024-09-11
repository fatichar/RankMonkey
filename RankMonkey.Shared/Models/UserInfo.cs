namespace RankMonkey.Shared.Models;

public class UserInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public FinanceInfo FinanceInfo { get; set; }
}