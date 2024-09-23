using Google.Apis.Auth;
using RankMonkey.Server.Pocos;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public class GoogleAuthProvider(
    IConfiguration configuration,
    UserService userService,
    JwtService jwtService,
    ILogger<GoogleAuthProvider> logger) : IAuthProvider
{
    private readonly string _googleClientId = configuration["Authentication:Google:ClientId"]
                                              ?? throw new Exception("Google Client ID not found");

    public AuthType AuthType => AuthType.Google;

    public async Task<Result<LoginResponse>> LoginAsync(string token)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string> { _googleClientId }
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

            var user = await userService.GetByExternalIdAsync(payload.Subject);
            if (user != null)
            {
                if (IsChanged(payload, user))
                {
                    user.Email = payload.Email;
                    user.Name = payload.Name;
                    await userService.UpdateUserAsync(user);
                }
            }
            else
            {
                var userDto = CreateDto(payload);
                user = await userService.CreateUserAsync(userDto);
            }

            var jwt = jwtService.GenerateToken(user);
            return Result.Success(new LoginResponse { Token = jwt });
        }
        catch (InvalidJwtException e)
        {
            logger.LogError(e, "Invalid Google token: {name}", token);
        }

        return Result.Failure<LoginResponse>("Invalid token");
    }

    public Task LogoutAsync()
    {
        return Task.CompletedTask;
    }

    // region Private Methods
    private static bool IsChanged(GoogleJsonWebSignature.Payload payload, UserDto existing)
    {
        return payload.Email != existing.Email || payload.Name != existing.Name;
    }
    private CreateUserDto CreateDto(GoogleJsonWebSignature.Payload payload)
    {
        return new CreateUserDto
        {
            Name = payload.Name,
            Email = payload.Email,
            AuthType = AuthType
        };
    }
    // endregion Private Methods
}