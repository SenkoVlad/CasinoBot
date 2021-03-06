using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Casino.Common.AppConstants;

namespace Casino.DAL.Models;

[Table("dbo.Chats")]
public class Chat
{
    [Key]
    public long Id { get; set; }
    public string Language { get; set; } = AppConstants.DefaultLanguage;
    public double Balance { get; set; } = 0;
    public int DemoBalance { get; set; } = AppConstants.DefaultBalance;
    public DateTime CreateDateTimeUtc { get; set; } = DateTime.UtcNow;
}