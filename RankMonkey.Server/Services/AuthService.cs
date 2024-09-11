using System.Security.Authentication;
using System.Security.Claims;
using AutoMapper;

using RankMonkey.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using RankMonkey.Server.Data;
using RankMonkey.Server.Entities;

namespace RankMonkey.Server.Services;

public class AuthService(ApplicationDbContext dbContext, JwtService jwtService, IMapper mapper)
{
    public async Task<string> OnAuthenticated(AuthenticateResult authenticateResult)
    {
        var claims = authenticateResult.Principal?.Identities.FirstOrDefault()?.Claims?.ToList();
        if (claims == null)
        {
            throw new AuthenticationException("No claims found in the authentication result.");
        }

        var userEmail = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(userEmail))
        {
            throw new AuthenticationException("No email found in the claims.");
        }

        // Check if the user exists in the database
        var user = dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
        if (user == null)
        {
            var googleId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var userName = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            user = await CreateUser(googleId, userName, userEmail);
        }

        var token = jwtService.GenerateToken(user);
        return token;
    }

    private async Task<User> CreateUser(string? googleId, string? userName, string userEmail)
    {
        User user;
        user = new User()
        {
            GoogleId = googleId,
            Name = userName,
            Email = userEmail
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public UserInfo? GetUser(int userId)
    {
        var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
        return user != null ? ToModel(user) : null;
    }

    private UserInfo ToModel(User user)
    {
        return mapper.Map<UserInfo>(user);
    }
}