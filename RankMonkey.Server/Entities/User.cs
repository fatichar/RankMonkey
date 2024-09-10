using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankMonkey.Server.Entities;

[Table("user")]
public class User
{
    [Key]
    [Column("id")]
    public string Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("phone_number")]
    public string PhoneNumber { get; set; }

    [Column("email")]
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Column("google_id")]
    public string GoogleId { get; set; }

    // public virtual Financials Financials { get; set; }
}