using System.Text.Json.Serialization;

namespace RankMonkey.Shared.Models;

public class TokenResponse(string token)
{
    [JsonPropertyName("token")]
    public string Token { get; } = token;
}