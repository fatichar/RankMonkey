using RankMonkey.Server.Pocos;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

class GuestAuthService(JwtService jwtService) : IAuthService
{
    public string Name => AuthType.GUEST;

    public async Task<Result<LoginResponse>> LoginAsync(string token)
    {
        var user = new UserDto(Guid.NewGuid().ToString(), "Guest", string.Empty)
        {
            Name = "Guest",
            Role = RoleNames.USER_ROLE_NAME
        };
        var jwt = jwtService.GenerateToken(user);
        return Result.Success(new LoginResponse { Token = jwt });
    }

    public Task LogoutAsync()
    {
        return Task.CompletedTask;
    }
}