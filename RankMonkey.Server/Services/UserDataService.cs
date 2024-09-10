using RankMonkey.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RankMonkey.Server.Data;

namespace RankMonkey.Server.Services;

public class UserDataService //: IUserDataService
{
    private readonly ApplicationDbContext _context;

    public UserDataService(ApplicationDbContext context)
    {
        _context = context;
    }

    // public async Task<Ranking> SubmitUserDataAsync(UserInfo userInfo)
    // {
    //     var existingData = await _context.Users.FindAsync(userInfo.Id);
    //     if (existingData != null)
    //     {
    //         existingData.Email = userInfo.Email;
    //         // existingData.Income = userInfo.Income;
    //         // existingData.NetWorth = userInfo.NetWorth;
    //     }
    //     else
    //     {
    //         _context.Users.Add(userInfo);
    //     }
    //
    //     await _context.SaveChangesAsync();
    //
    //     var incomePercentile = await CalculatePercentile(userInfo.Income, u => u.Income);
    //     var netWorthPercentile = await CalculatePercentile(userInfo.NetWorth, u => u.NetWorth);
    //
    //     return new Ranking
    //     {
    //         IncomePercentile = incomePercentile,
    //         NetWorthPercentile = netWorthPercentile
    //     };
    // }
    //
    // public async Task<UserInfo> GetUserDataAsync(string userId)
    // {
    //     return await _context.Users.FindAsync(userId);
    // }
    //
    // private async Task<int> CalculatePercentile(int value, Expression<Func<UserInfo, int>> selector)
    // {
    //     var totalCount = await _context.Users.CountAsync();
    //     var lowerCount = await _context.Users.CountAsync(u => selector.Compile()(u) < value);
    //     return (int)Math.Round((double)lowerCount / totalCount * 100);
    // }
}