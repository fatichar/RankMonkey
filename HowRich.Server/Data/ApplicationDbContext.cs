using HowRich.Server.Entities;
using Microsoft.EntityFrameworkCore;
namespace HowRich.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public required DbSet<User> Users { get; set; }
}