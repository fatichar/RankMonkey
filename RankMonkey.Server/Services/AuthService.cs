using AutoMapper;
using Google.Apis.Auth;
using RankMonkey.Shared.Models;
using RankMonkey.Server.Data;
using Microsoft.EntityFrameworkCore;
using RankMonkey.Server.Entities;

namespace RankMonkey.Server.Services;

public class AuthService(IConfiguration configuration, ApplicationDbContext context, IMapper mapper)
{
    public async Task<UserInfo?> VerifyGoogleToken(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { configuration["Authentication:Google:ClientId"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);
            if (user == null)
            {
                user = new User
                {
                    Email = payload.Email,
                    Name = payload.Name,
                    GoogleId = payload.Subject
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            return ToModel(user);
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }

    public UserInfo GetUser(string userId)
    {
        var guidUserId = Guid.Parse(userId);
        var user = context.Users.FirstOrDefault(u => u.Id == guidUserId);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        return ToModel(user);
    }

    private UserInfo ToModel(User user)
    {
        return mapper.Map<UserInfo>(user);
    }
}