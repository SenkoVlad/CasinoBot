using Casino.Common.AppConstants;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ButtonsGenerators;

public class KeyboardButtonsGenerator
{
    private ReplyKeyboardMarkup _replyKeyboardMarkup;
    private string[] _availableCommands = {};

    public ReplyKeyboardMarkup GetReplyKeyboardMarkup => _replyKeyboardMarkup;
    public string[] GetAvailableCommands => _availableCommands;
    public void InitStartButtons()
    {
        var startGameButton = new KeyboardButton(ButtonTextConstants.StartGameButtonText);
        var getInfoButton = new KeyboardButton(ButtonTextConstants.GetInfoButtonText);

        var buttons = new[] { startGameButton, getInfoButton };
        _replyKeyboardMarkup = new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };

        _availableCommands = new[]
        {
            ButtonTextConstants.StartGameButtonText, 
            ButtonTextConstants.GetInfoButtonText
        };
    }

    public void InitGamesButtons()
    {
        var diceGameButton = new KeyboardButton(ButtonTextConstants.DiceGameButtonText);
        var russianRouletteGameButton = new KeyboardButton(ButtonTextConstants.RussianRouletteGameButtonText);
        var backButton = new KeyboardButton(ButtonTextConstants.BackButtonText);

        var buttons = new[] { diceGameButton, russianRouletteGameButton, backButton};
        _replyKeyboardMarkup = new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };
        _availableCommands = new[]
        {
            ButtonTextConstants.RussianRouletteGameButtonText, 
            ButtonTextConstants.DiceGameButtonText,
            ButtonTextConstants.BackButtonText
        };
    }

    public void InitDiceGameButtons()
    {
        var diceGameButton = new KeyboardButton(ButtonTextConstants.StartDiceGameButtonText);
        var backButton = new KeyboardButton(ButtonTextConstants.BackButtonText);

        var buttons = new[] { diceGameButton, backButton };
        _replyKeyboardMarkup = new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };

        _availableCommands = new[]
        {
            ButtonTextConstants.StartDiceGameButtonText, 
            ButtonTextConstants.BackButtonText
        };
    }
}