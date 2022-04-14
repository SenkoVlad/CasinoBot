using Casino.BLL.ButtonsGenerators;
using Casino.Common.AppConstants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Casino.BLL.Games;

public class FootBallGame : Game
{
    private readonly Message _message;
    private readonly ITelegramBotClient _telegramBotClient;
    private int? _scoreResult;
    private int _goodLuckMessageId;

    public FootBallGame(Message message,
        ITelegramBotClient telegramBotClient)
    {
        _message = message;
        _telegramBotClient = telegramBotClient;
    }

    protected override async Task InitGameAsync()
    {
        var inlineKeyboardButtonsGenerator = new InlineKeyboardButtonsGenerator();
        inlineKeyboardButtonsGenerator.InitFootballButtons();
        var inlineKeyboardButtons = inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        var footballGameButtonText = ButtonTextConstants.FootballGameButtonText;

        await _telegramBotClient.SendTextMessageAsync(_message.Chat.Id, text: footballGameButtonText,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override async Task SentStartMessageAsync()
    {
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId,
            MessageTextConstants.GoodLuckFootBallMessageText);
        _goodLuckMessageId = goodLuckMessage.MessageId;
    }

    protected override async Task PlayGameRoundAsync()
    {
        var hitResult = await _telegramBotClient.SendDiceAsync(_message.Chat.Id, Emoji.Football);
        _scoreResult = hitResult.Dice?.Value;
    }

    protected override async Task SendRoundResultAsync()
    {
        var roundResultMessage = IsItGoal() ? "GOAL!" : "MISS ((";

        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }

    private bool IsItGoal()
    {
        return ReplyConstants.GoalScores.Contains((int)_scoreResult!);
    }
}