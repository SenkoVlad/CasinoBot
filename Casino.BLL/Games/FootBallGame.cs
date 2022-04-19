using Casino.BLL.ButtonsGenerators;
using Casino.Common.AppConstants;
using Casino.DAL.Repositories.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Casino.BLL.Games;

public class FootBallGame : Game
{
    private readonly long _chatId;
    private readonly int _messageId;
    private readonly IBalanceRepository _balanceRepository;
    private readonly ITelegramBotClient _telegramBotClient;
    private int? _scoreResult;
    private int _goodLuckMessageId;

    public FootBallGame(long chatId,
        int messageId,
        ITelegramBotClient telegramBotClient,
        IBalanceRepository balanceRepository) : base(chatId, balanceRepository)
    {
        _chatId = chatId;
        _telegramBotClient = telegramBotClient;
        _balanceRepository = balanceRepository;
        _messageId = messageId;
    }

    protected override async Task InitGameAsync()
    {
        var inlineKeyboardButtonsGenerator = new InlineKeyboardButtonsGenerator();
        inlineKeyboardButtonsGenerator.InitPlayFootballDemoButtons();
        var inlineKeyboardButtons = inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        var balance = _balanceRepository.GetBalanceAsync(_chatId);
        var footballGameButtonText = $"{MessageTextConstants.GoodLuckFootBallMessageText}. Your balance: {balance}";
        await _telegramBotClient.SendTextMessageAsync(_chatId, text: footballGameButtonText,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override async Task SentStartMessageAsync()
    {
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_chatId, _messageId,
            MessageTextConstants.GoodLuckFootBallMessageText);
        _goodLuckMessageId = goodLuckMessage.MessageId;
    }

    protected override async Task PlayGameRoundAsync()
    {
        var hitResult = await _telegramBotClient.SendDiceAsync(_chatId, Emoji.Football);
        _scoreResult = hitResult.Dice?.Value;
    }

    protected override bool GetRoundResult()
    {
        return ReplyConstants.GoalScores.Contains((int)_scoreResult!);
    }

    protected override async Task SendRoundResultMessageAsync()
    {
        var roundResultMessage = DidWin ? "GOAL!" : "MISS ((";

        await _telegramBotClient.EditMessageTextAsync(_chatId, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}