using System.Text.Json.Serialization;

namespace RankMonkey.Shared.Models;

public class UserDto(string id, string name, string email)
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = id;

    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("email")]
    public string Email { get; set; } = email;

    [JsonPropertyName("role")]
    public required string Role { get; set; }
}