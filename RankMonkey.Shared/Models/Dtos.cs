namespace RankMonkey.Shared.Models;

public record GoogleLoginRequest(string IdToken);

public record LoginResponse(string Token);

public record CreateUserRequest(
    string Name,
    string Email,
    AuthType AuthType,
    string? ExternalId = null);

public record UpdateUserRequest(string Id, string Name, string Email);

public record UserDto(string Id, string Name, string Email)
{
    public required string Role { get; set; }
}

public record UpdateRoleRequest(string Role);