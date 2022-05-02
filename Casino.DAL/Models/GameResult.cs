using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casino.DAL.Models;

[Table("dbo.GameResults")]
public class GameResult
{
    [Key]
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int Bet { get; set; }
    public int BettingResultId { get; set; }
}