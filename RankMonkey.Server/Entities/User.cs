using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Entities;

[Table("user")]
public class User
{
    // ReSharper disable once ConvertConstructorToMemberInitializers
    public User()
    {
        IsActive = true;
        IsDummy = false;
    }

    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    [Column("name")]
    [StringLength(128, MinimumLength = 2)]
    public required string Name { get; set; }

    [Column("email")]
    [EmailAddress]
    [Required]
    [StringLength(256)]
    public required string Email { get; set; }

    [Column("role_name")]
    [StringLength(32, MinimumLength = 3)]
    public required string RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; init; } = null!;

    [Column("is_dummy")]
    public bool IsDummy { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; init; }

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    [Column("auth_type")]
    public AuthType AuthType { get; set; }

    [Column("external_id")]
    [StringLength(256)]
    public string? ExternalId { get; set; }
}