using AutoMapper;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using RankMonkey.Server.Data;
using RankMonkey.Server.Entities;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Services;

public class UserService(ApplicationDbContext context, IMapper mapper)
{
    public UserDto GetUser(string userId)
    {
        var guidUserId = Guid.Parse(userId);
        var user = context.Users.FirstOrDefault(u => u.Id == guidUserId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        return ToModel(user);
    }

    public async Task<UserDto?> UpdateUserRole(Guid userId, string newRole)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return null;

        if (!IsValidRole(newRole))
            throw new ArgumentException("Invalid role specified");

        user.RoleId = Guid.Parse(newRole);
        await context.SaveChangesAsync();

        return ToModel(user);
    }

    public async Task<UserDto?> GetUserById(Guid userId)
    {
        var user = await context.Users.FindAsync(userId);
        return user != null ? ToModel(user) : null;
    }

    private bool IsValidRole(string role)
    {
        return context.Roles.FirstOrDefault(r => r.Name == role) != null;
    }

    public async Task<UserDto?> GetUserAsync(GoogleJsonWebSignature.Payload payload)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

        return user == null ? null : ToModel(user);
    }

    public async Task<UserDto> CreateUserAsync(GoogleJsonWebSignature.Payload payload)
    {
        var now = DateTime.UtcNow;
        var user = new User
        {
            Email = payload.Email,
            Name = payload.Name,
            GoogleId = payload.Subject,
            RoleId = await SuggestRoleId(context),
            IsActive = true,
            CreatedAt = now,
            LastLoginAt = now
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return ToModel(user);
    }

    private static async Task<Guid> SuggestRoleId(ApplicationDbContext context)
    {
        var isFirstUser = !await context.Users.AnyAsync();
        var roleName = isFirstUser ? "Admin" : "User";
        return Guid.Parse(roleName);
    }

    private UserDto ToModel(User user)
    {
        return mapper.Map<UserDto>(user);
    }
}