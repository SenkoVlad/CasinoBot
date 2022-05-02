using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casino.DAL.Models;

[Table("dbo.BettingResults")]
public class BettingResult
{
    [Key]
    public int Id { get; set; }
    public int GameId { get; set; }
    public double Coefficient { get; set; }
    public bool IsWin { get; set; }
}