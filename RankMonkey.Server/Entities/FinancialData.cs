using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankMonkey.Server.Entities;

[Table("financial_data")]
public class FinancialData
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [Column("data_type")]
    public string DataType { get; set; } = null!;

    [Column("value")]
    public long Value { get; set; }

    [Column("timestamp")]
    public DateTime Timestamp { get; set; }
}