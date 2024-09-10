using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public class AuthenticationService(IEnumerable<IAuthProvider> authServices, GuestAuthProvider? guestAuthProvider)
{
    public async Task<Result<LoginResponse>> LoginAsync(AuthType authType, string token)
    {
        var authService = authServices.FirstOrDefault(x => x.AuthType == authType);
        if (authService == null)
        {
            throw new ArgumentException($"No auth service found for {authType}");
        }

        return await authService.LoginAsync(token);
    }

    public Result<LoginResponse> LoginAsGuest()
    {
        return guestAuthProvider?.Login() ?? throw new Exception("Guest auth provider not found");
    }
}