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

        var buttonRows = new[]{ getInfoButton, balanceButton, gamesButton };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
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
        var footballGameButton = new InlineKeyboardButton(ButtonTextConstants.FootballGameButtonText)
        {
            CallbackData = ButtonTextConstants.FootballGameButtonText
        };
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = ButtonTextConstants.MenuButtonText
        };

        var buttonRows = new[] 
        {
            new[] { footballGameButton, russianRouletteGameButton, diceGameButton },
            new[] { backButton}
        };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitGetBalanceButtons()
    {
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = ButtonTextConstants.MenuButtonText
        };

        var buttonRows = new[] { backButton };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitFootballButtons()
    {
        var hitBallButton = new InlineKeyboardButton(ButtonTextConstants.HitBallButtonText)
        {
            CallbackData = ButtonTextConstants.HitBallButtonText
        };
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = ButtonTextConstants.BackFromFootballButtonCallbackData
        };

        var buttonRows = new[] { hitBallButton, backButton };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }
}