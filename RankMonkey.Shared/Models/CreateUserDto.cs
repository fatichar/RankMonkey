namespace RankMonkey.Shared.Models;

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public AuthType AuthType { get; set; }
    public string? ExternalId { get; set; }
}