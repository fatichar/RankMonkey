using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankMonkey.Server.Entities;

[Table("metrics")]
public class Metrics
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [Column("income")]
    public long Income { get; set; }

    [Column("net_worth")]
    public long NetWorth { get; set; }

    [Column("currency")]
    [StringLength(3)]
    public string Currency { get; set; } = "INR";

    [Column("timestamp")]
    public DateTime Timestamp { get; set; }
}