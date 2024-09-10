using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public class GuestAuthProvider(JwtService jwtService)
{
    public Result<LoginResponse> Login()
    {
        var user = new UserDto(Guid.NewGuid().ToString(), "Guest", string.Empty)
        {
            Name = "Guest",
            Role = Roles.USER
        };
        var jwt = jwtService.GenerateToken(user);
        return Result.Success(new LoginResponse { Token = jwt });
    }

    public void Logout()
    {
    }
}