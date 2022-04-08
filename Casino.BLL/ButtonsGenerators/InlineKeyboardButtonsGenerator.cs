using Casino.Common.AppConstants;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ButtonsGenerators;

public class InlineKeyboardButtonsGenerator
{
    private InlineKeyboardMarkup _inlineKeyboardMarkup;
    public InlineKeyboardMarkup GetInlineKeyboardMarkup => _inlineKeyboardMarkup;

    public void InitStartButtons()
    {
        var gamesButton = new InlineKeyboardButton(ButtonTextConstants.GamesButtonText)
        {
            CallbackData = ButtonTextConstants.GamesButtonText
        };
        var getInfoButton = new InlineKeyboardButton(ButtonTextConstants.GetInfoButtonText)
        {
            CallbackData = ButtonTextConstants.GetInfoButtonText
        };
        var balanceButton = new InlineKeyboardButton(ButtonTextConstants.BalanceButtonText)
        {
            CallbackData = ButtonTextConstants.BalanceButtonText
        };

        var buttonRow = new[]{ getInfoButton, balanceButton, gamesButton };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRow);
    }

    public void InitGamesButtons()
    {
        var diceGameButton = new InlineKeyboardButton(ButtonTextConstants.DiceGameButtonText)
        {
            CallbackData = ButtonTextConstants.DiceGameButtonText
        };
        var russianRouletteGameButton = new InlineKeyboardButton(ButtonTextConstants.RussianRouletteGameButtonText)
        {
            CallbackData = ButtonTextConstants.RussianRouletteGameButtonText
        };
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = ButtonTextConstants.MenuButtonText
        };

        var buttonRow = new[] { russianRouletteGameButton, diceGameButton, backButton};
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRow);
    }

    public void InitGetBalanceButtons()
    {
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = ButtonTextConstants.MenuButtonText
        };

        var buttonRow = new[] { backButton };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRow);
    }
}