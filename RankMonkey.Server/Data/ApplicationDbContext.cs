using Microsoft.EntityFrameworkCore;
using RankMonkey.Server.Entities;

namespace RankMonkey.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public required DbSet<User> Users { get; set; }
}