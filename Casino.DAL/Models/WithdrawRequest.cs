using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casino.DAL.Models;

[Table("dbo.WithdrawRequests")]
public class WithdrawRequest
{
    [Key]
    public int Id { get; set; }

    public long ChatId { get; set; }
    public int CurrencyId { get; set; }
    public int Amount { get; set; }
    public bool IsAccounted { get; set; }
    public DateTime CreatedDateTimeUtc { get; set; }
}