using Microsoft.EntityFrameworkCore;
using RankMonkey.Server.Data;
using RankMonkey.Server.Entities;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public class FinancialDataService(ApplicationDbContext context)
{
    public async Task<Result<FinancialDataDto>> AddOrUpdateFinancialDataAsync(Guid userId, string dataType, long value)
    {
        var existingData = await context.FinancialData
            .FirstOrDefaultAsync(fd => fd.UserId == userId && fd.DataType == dataType);

        if (existingData == null)
        {
            existingData = new FinancialData
            {
                UserId = userId,
                DataType = dataType,
                Value = value,
                Timestamp = DateTime.UtcNow
            };
            context.FinancialData.Add(existingData);
        }
        else
        {
            existingData.Value = value;
            existingData.Timestamp = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();

        return Result.Success(new FinancialDataDto
        {
            Id = existingData.Id,
            UserId = existingData.UserId,
            DataType = existingData.DataType,
            Value = existingData.Value,
            Timestamp = existingData.Timestamp
        });
    }

    public async Task<Result<int>> GetRankingAsync(string dataType, decimal value)
    {
        var rank = await context.FinancialData
            .Where(fd => fd.DataType == dataType && fd.Value > value)
            .CountAsync();

        return Result.Success(rank + 1);
    }

    public async Task<Result<List<FinancialDataDto>>> GetUserFinancialDataAsync(Guid userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
        {
            return Result.Failure<List<FinancialDataDto>>("User not found.");
        }

        var financialData = await context.FinancialData
            .Where(fd => fd.UserId == userId)
            .Select(fd => new FinancialDataDto
            {
                Id = fd.Id,
                UserId = fd.UserId,
                DataType = fd.DataType,
                Value = fd.Value,
                Timestamp = fd.Timestamp
            })
            .ToListAsync();

        return Result.Success(financialData);
    }

    public async Task<Result<string>> GetUserCurrencyAsync(Guid userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
        {
            return Result.Failure<string>("User not found.");
        }

        return Result.Success(user.Currency);
    }
}