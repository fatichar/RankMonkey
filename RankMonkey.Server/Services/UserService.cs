using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RankMonkey.Server.Data;
using RankMonkey.Server.Entities;
using RankMonkey.Server.Pocos;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public class UserService(ApplicationDbContext context, IMapper mapper)
{
    // region Public Methods
    // region Create
    public async Task<UserDto> CreateUserAsync(CreateUserDto newUser)
    {
        var now = DateTime.UtcNow;
        var user = new User
        {
            Email = newUser.Email,
            Name = newUser.Name,
            ExternalId = newUser.ExternalId,
            RoleId = await SuggestRole(context),
            CreatedAt = now,
            LastLoginAt = now,
            AuthType = newUser.AuthType
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return ToModel(user);
    }
    // endregion Create

    // region Get
    public async Task<UserDto?> GetById(Guid userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new Exception($"User with id {userId} not found");
        }
        return ToModel(user);
    }

    public async Task<UserDto?> GetByExternalIdAsync(string externalId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.ExternalId == externalId);
        if (user == null)
            return null;

        return ToModel(user);
    }
    // endregion Get

    // region Update
    public async Task<Result<UserDto>> UpdateUserAsync(UserDto user)
    {
        var existingUser = await context.Users.FindAsync(user.Id);
        if (existingUser == null)
            return Result.Failure<UserDto>("User not found");

        existingUser.Email = user.Email;
        existingUser.Name = user.Name;
        existingUser.RoleId = user.Role;

        context.Users.Update(existingUser);
        await context.SaveChangesAsync();

        return Result.Success(ToModel(existingUser));
    }

    public async Task<UserDto?> UpdateUserRole(Guid userId, string newRole)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return null;

        if (!IsValidRole(newRole))
            throw new ArgumentException("Invalid role specified");

        user.RoleId = newRole;
        await context.SaveChangesAsync();

        return ToModel(user);
    }
    // endregion Update
    // endregion Public Methods

    // region Private Methods
    private bool IsValidRole(string role)
    {
        return context.Roles.Any(r => r.Id == role);
    }

    private static async Task<string> SuggestRole(ApplicationDbContext context)
    {
        var isFirstUser = !await context.Users.AnyAsync();
        var roleName = isFirstUser ? Roles.ADMIN : Roles.USER;
        return roleName;
    }
    // endregion Private Methods

    // region Helper Methods
    private UserDto ToModel(User user) => mapper.Map<UserDto>(user);
    // endregion Helper Methods
}