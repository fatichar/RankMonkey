using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankMonkey.Server.Entities;

[Table("user")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("email")]
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Column("google_id")]
    [Required]
    public string GoogleId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    [Column("role_id")]
    public Guid? RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; set; }
}