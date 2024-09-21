using Microsoft.EntityFrameworkCore;
using RankMonkey.Server.Entities;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
    public required DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        SeedRoles(modelBuilder);
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        var adminRole = new Role
        {
            Name = RoleNames.ADMIN_ROLE_NAME,
            Description = "Admin role"
        };

        var userRole = new Role
        {
            Name = RoleNames.USER_ROLE_NAME,
            Description = "Default user role"
        };

        modelBuilder.Entity<Role>().HasData(
            adminRole,
            userRole
        );
    }
}