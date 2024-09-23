using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankMonkey.Server.Entities;

[Table("role")]
public class Role
{
    [Key]
    [Column("id")]
    [Required]
    [StringLength(32, MinimumLength = 3)]
    public required string Id { get; init; }

    [Column("name")]
    [Required]
    [StringLength(32, MinimumLength = 3)]
    public required string Name { get; init; }

    [Column("description")]
    [StringLength(256)]
    public string? Description { get; init; }
}