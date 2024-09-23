using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public interface IAuthProvider
{
    AuthType AuthType { get; }
    Task<Result<LoginResponse>> LoginAsync(string token);
    Task LogoutAsync();
}