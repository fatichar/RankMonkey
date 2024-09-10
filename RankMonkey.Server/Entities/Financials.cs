using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Entities;

[Table("financials")]
public class Financials
{
    [Key]
    [Column("id")]
    public string UserId { get; set; }

    [Column("currency")]
    [MaxLength(3)]  // ISO 4217 standard for currency codes (e.g., USD, EUR)
    public string Currency { get; set; }

    [Required]
    [Column("income")]
    public int Income { get; set; }

    [Required]
    [Column("net_worth")]
    public int NetWorth { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}