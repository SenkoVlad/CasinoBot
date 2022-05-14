namespace Casino.BLL.Models;

public class GameModel
{
    public bool IsDemoPlay { get; set; }
    public int UserBet { get; set; }
    public ChatModel Chat { get; set; } = null!;
    public int GameId { get; set; }
    public BettingResultModel BettingResult { get; set; } = null!;
    public int DiceResult { get; set; }
}