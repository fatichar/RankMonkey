using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public interface IUserDataService
{
    Task<UserInfo> GetUserDataAsync(string userId);
}