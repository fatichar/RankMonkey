using Microsoft.EntityFrameworkCore;
using RankMonkey.Server.Entities;

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
        var userRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "User",
            Description = "Default user role"
        };

        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Description = "Admin role"
        };

        modelBuilder.Entity<Role>().HasData(
            adminRole,
            userRole
        );
    }
}