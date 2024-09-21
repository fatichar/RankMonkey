using System.Text.Json.Serialization;

namespace RankMonkey.Shared.Models;

public class LoginResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}