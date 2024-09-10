using System.Text.Json.Serialization;

namespace RankMonkey.Shared.Models;

public class GoogleLoginRequest(string idToken)
{
    [JsonPropertyName("idToken")]
    public string IdToken { get; set; } = idToken;
}