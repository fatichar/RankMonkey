using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RankMonkey.Server.Entities;

namespace RankMonkey.Server.Services;

public class JwtService
{
    private const int DEFAULT_TOKEN_EXPIRATION_IN_HOURS = 1;

    private readonly SigningCredentials _credentials;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _tokenExpirationInHours;

    public JwtService(IConfiguration configuration)
    {
        var keyData = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException());
        var key = new SymmetricSecurityKey(keyData);
        _credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        _issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException();
        _audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException();

        _tokenExpirationInHours = configuration.GetValue<int?>("Jwt:TokenExpirationInHours")
                                  ?? DEFAULT_TOKEN_EXPIRATION_IN_HOURS;
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddHours(_tokenExpirationInHours),
            signingCredentials: _credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}