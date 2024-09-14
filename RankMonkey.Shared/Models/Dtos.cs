using System.Text.Json.Serialization;

namespace RankMonkey.Shared.Models;

public class GoogleLoginRequest
{
    public string IdToken { get; set; } = string.Empty;
}

public class UpdateRoleRequest
{
    public string NewRole { get; set; } = string.Empty;
}

public class UserDto(string id, string name, string email)
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = id;

    [JsonPropertyName("name")]
    public string Name { get; init; } = name;

    [JsonPropertyName("email")]
    public string Email { get; init; } = email;

    [JsonPropertyName("role")]

    public string Role { get; set; }
}

public class TokenResponse(string token)
{
    [JsonPropertyName("token")]
    public string Token { get; } = token;
}