namespace Casino.BLL.Models;

public class GameModel
{
    public bool IsDemoPlay { get; set; }
    public int UserBet { get; set; }
    public bool DidWin { get; set; }
    public ChatModel Chat { get; set; } = null!;
}