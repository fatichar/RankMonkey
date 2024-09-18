using AutoMapper;
using Google.Apis.Auth;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public class AuthService(
    IConfiguration configuration,
    UserService userService,
    ILogger<AuthService> logger)
{
    private readonly string _googleClientId = configuration["Authentication:Google:ClientId"]
                                              ?? throw new Exception("Google Client ID not found");

    public async Task<UserDto?> VerifyGoogleToken(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _googleClientId }
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return await userService.GetUserAsync(payload)
                       ?? await userService.CreateUserAsync(payload);
            }
            catch (InvalidJwtException e)
            {
                logger.LogError(e, "Invalid Google token: {name}", idToken);
            }

            return null;
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}