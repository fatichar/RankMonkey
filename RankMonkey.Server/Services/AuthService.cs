using System.Security.Claims;
using RankMonkey.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using RankMonkey.Server.Data;
using RankMonkey.Server.Entities;

namespace RankMonkey.Server.Services;

public class AuthService(ApplicationDbContext dbContext)
{
    public async Task<UserInfo> OnAuthenticated(AuthenticateResult authenticateResult)
    {
        var claims = authenticateResult.Principal?.Identities.FirstOrDefault()?.Claims;
        var userId = claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var userName = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        var userEmail = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        var user = new User()
        {
            GoogleId = userId,
            Name = userName,
            Email = userEmail
        };

        // Check if the user exists in the database
        var existingUser = dbContext.Users.FirstOrDefault(u => u.GoogleId == userId);
        if (existingUser == null)
        {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            existingUser = user;
        }

        return ToModel(existingUser);
    }

    private static UserInfo ToModel(User user)
    {
        return new UserInfo()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}