using Casino.Common.Enum;

namespace Casino.BLL.Models;

public class InitGameModel
{
    public Command PlayGameCommand { get; set; }
    public Command ChangeGameBetCommand { get; set; }
    public string PlayButtonText { get; set; }
    public Command PlayToCommand { get; set; }
}