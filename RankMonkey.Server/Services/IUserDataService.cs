using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public interface IUserDataService
{
    Task<Ranking> SubmitUserDataAsync(UserInfo userInfo);
    Task<UserInfo> GetUserDataAsync(string userId);
}