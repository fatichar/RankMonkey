using Microsoft.EntityFrameworkCore;
using RankMonkey.Server.Entities;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; init; }
    public required DbSet<Role> Roles { get; init; }
    public required DbSet<FinancialData> FinancialData { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>(role =>
        {
            role.HasIndex(x => x.Name).IsUnique();
            role.Property(x => x.Id).IsRequired().HasMaxLength(32);
            role.Property(x => x.Name).IsRequired().HasMaxLength(32);
            role.Property(x => x.Description).HasMaxLength(256);
        });

        modelBuilder.Entity<User>(user =>
        {
            user.HasIndex(x => x.Email).IsUnique();
            user.Property(x => x.Name).IsRequired().HasMaxLength(128);
            user.Property(x => x.Email).IsRequired().HasMaxLength(256);
            user.Property(x => x.RoleId).IsRequired().HasMaxLength(32);
            user.Property(x => x.ExternalId).HasMaxLength(256);

            user.Property(x => x.IsActive).HasDefaultValue(true);
            user.Property(x => x.IsDummy).HasDefaultValue(false);
            user.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            user.Property(x => x.AuthType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
            var authTypes = Enum.GetNames(typeof(AuthType));
            var authTypesString = string.Join(",", authTypes.Select(t => $"'{t}'"));

            user.ToTable(t => t.HasCheckConstraint("CK_Users_AuthType", $"auth_type IN ({authTypesString})"));
            user.HasOne(x => x.Role)
                  .WithMany()
                  .HasForeignKey(x => x.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FinancialData>(financialData =>
        {
            financialData.HasIndex(x => new { x.UserId, x.DataType }).IsUnique();
            financialData.Property(x => x.DataType).HasMaxLength(50);
            financialData.Property(x => x.Value).HasPrecision(18, 2);
        });

        SeedRoles(modelBuilder);
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        var adminRole = new Role
        {
            Id = Shared.Models.Roles.ADMIN,
            Name = "Admin",
            Description = "Admin role"
        };

        var userRole = new Role
        {
            Id = Shared.Models.Roles.USER,
            Name = "User",
            Description = "Default user role"
        };

        modelBuilder.Entity<Role>().HasData(
            adminRole,
            userRole
        );
    }
}