using System.Security.Claims;
using System.Text.Json;

namespace RankMonkey.Client.Auth;

public class JwtUtil
{
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var pairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (pairs == null) return claims;

        pairs.TryGetValue(ClaimTypes.Role, out object? roles);

        if (roles != null)
        {
            var rolesString = roles.ToString()!.Trim();
            if (rolesString.StartsWith('['))
            {
                var parsedRoles = JsonSerializer.Deserialize<string[]>(rolesString)!;

                foreach (var role in parsedRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, rolesString));
            }

            pairs.Remove(ClaimTypes.Role);
        }

        claims.AddRange(pairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}