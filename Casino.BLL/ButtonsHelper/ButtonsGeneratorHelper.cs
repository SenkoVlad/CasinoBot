using Casino.Common.AppConstants;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ButtonsHelper;

public class ButtonsGeneratorHelper
{
    public static ReplyKeyboardMarkup GetStartButtons()
    {
        var startGameButton = new KeyboardButton(ButtonTextConstants.StartGameButtonText);
        var getInfoButton = new KeyboardButton(ButtonTextConstants.GetInfoButtonText);

        var buttons = new[] { startGameButton, getInfoButton};
        var replyKeyboardMarkup = new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };
        
        return replyKeyboardMarkup;
    }

    public static ReplyKeyboardMarkup GetGamesButtons()
    {
        var diceGameButton = new KeyboardButton(ButtonTextConstants.DiceGameButtonText);
        var russianRouletteGameButton = new KeyboardButton(ButtonTextConstants.RussianRouletteGameButtonText);
        var backButton = new KeyboardButton(ButtonTextConstants.BackButtonText);

        var buttons = new[] { diceGameButton, russianRouletteGameButton, backButton};
        var replyKeyboardMarkup = new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };

        return replyKeyboardMarkup;
    }
}