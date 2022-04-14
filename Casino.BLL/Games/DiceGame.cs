using Casino.BLL.ButtonsGenerators;
using Casino.Common.AppConstants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Casino.BLL.Games;

public class DiceGame : Game
{
    private readonly Message _message;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly int _diceBet;
    private int? _scoreResult;
    private int _goodLuckMessageId;

    public DiceGame(Message message, 
        ITelegramBotClient telegramBotClient,
        int diceBet)
    {
        _message = message;
        _telegramBotClient = telegramBotClient;
        _diceBet = diceBet;
    }

    protected override async Task InitGameAsync()
    {
        var inlineKeyboardButtonsGenerator = new InlineKeyboardButtonsGenerator();
        inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons();
        var chooseYourBetMessage = MessageTextConstants.ChooseYourBet;
        var inlineKeyboardButtons = inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.SendTextMessageAsync(_message.Chat.Id, text: chooseYourBetMessage,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override async Task SentStartMessageAsync()
    {
        var startMessage = $"{MessageTextConstants.GoodLuckFootBallMessageText}. Your bet is {_diceBet}";
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId, startMessage);
        _goodLuckMessageId = goodLuckMessage.MessageId;
    }

    protected override async Task PlayGameRoundAsync()
    {
        var diceResult = await _telegramBotClient.SendDiceAsync(_message.Chat.Id, Emoji.Dice);
        _scoreResult = diceResult.Dice?.Value;
    }

    protected override async Task SendRoundResultAsync()
    {
        var roundResultMessage = _scoreResult == _diceBet ? "Win" : "Lose";
        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}