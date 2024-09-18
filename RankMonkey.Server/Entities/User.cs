using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankMonkey.Server.Entities;

[Table("user")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; init; }

    [Column("name")]
    [StringLength(64, MinimumLength = 2)]
    public required string Name { get; set; }

    [Column("email")]
    [EmailAddress]
    [Required]
    [StringLength(256)]
    public required string Email { get; set; }

    [Column("role_name")]
    [StringLength(32, MinimumLength = 3)]
    public required string RoleName { get; set; }

    [ForeignKey("RoleName")]
    public Role Role { get; set; } = null!;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; init; }

    [Column("last_login_at")]
    public DateTime LastLoginAt { get; set; }
}