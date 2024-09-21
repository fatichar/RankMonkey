using RankMonkey.Server.Pocos;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public interface IAuthService
{
    string Name { get; }
    Task<Result<LoginResponse>> LoginAsync(string token);
    Task LogoutAsync();
}