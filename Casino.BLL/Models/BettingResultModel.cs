namespace Casino.BLL.Models;

public class BettingResultModel
{
    public int Id { get; set; }
    public double Coefficient { get; set; }
    public bool IsWon { get; set; }
}