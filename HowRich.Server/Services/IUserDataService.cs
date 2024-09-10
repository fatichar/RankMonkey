using HowRich.Shared.Models;

namespace HowRich.Server.Services;

public interface IUserDataService
{
    Task<Ranking> SubmitUserDataAsync(UserInfo userInfo);
    Task<UserInfo> GetUserDataAsync(string userId);
}