using Casino.BLL.ButtonsGenerators;
using Casino.BLL.ClickHandlers.Implementation;
using Casino.Common.AppConstants;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Casino.BLL.Games;

public class FootBallGame : Game
{
    private readonly long _chatId;
    private readonly int _messageId;
    private readonly IBalanceRepository _balanceRepository;
    private readonly InlineKeyboardButtonsGenerator _inlineKeyboardButtonsGenerator;
    private readonly IStringLocalizer<ButtonClickHandler> _localizer;
    private readonly ITelegramBotClient _telegramBotClient;
    private int? _scoreResult;
    private int _goodLuckMessageId;

    private static readonly int[] GoalScores = { 5, 3, 4 };
    
    public FootBallGame(long chatId,
        int messageId,
        ITelegramBotClient telegramBotClient,
        IBalanceRepository balanceRepository,
        int userBet,
        InlineKeyboardButtonsGenerator inlineKeyboardButtonsGenerator,
        IStringLocalizer<ButtonClickHandler> localizer) : base(chatId, balanceRepository, userBet)
    {
        _chatId = chatId;
        _telegramBotClient = telegramBotClient;
        _balanceRepository = balanceRepository;
        _inlineKeyboardButtonsGenerator = inlineKeyboardButtonsGenerator;
        _localizer = localizer;
        _messageId = messageId;
    }

    protected override async Task InitGameAsync()
    {
        _inlineKeyboardButtonsGenerator.InitPlayFootballDemoButtons(UserBet);
        var inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        var balance = _balanceRepository.GetBalanceAsync(_chatId);
        var footballGameButtonText = $"{_localizer[ResourceConstants.GoodLuckFootBallMessageText]}. Your balance: {balance}";
        await _telegramBotClient.SendTextMessageAsync(_chatId, text: footballGameButtonText,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override async Task SentStartMessageAsync()
    {
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_chatId, _messageId,
            _localizer[ResourceConstants.GoodLuckFootBallMessageText]);
        _goodLuckMessageId = goodLuckMessage.MessageId;
    }

    protected override async Task PlayGameRoundAsync()
    {
        var hitResult = await _telegramBotClient.SendDiceAsync(_chatId, Emoji.Football);
        _scoreResult = hitResult.Dice?.Value;
    }

    protected override bool GetRoundResult()
    {
        return GoalScores.Contains((int)_scoreResult!);
    }

    protected override async Task SendRoundResultMessageAsync()
    {
        var roundResultMessage = DidWin ? "GOAL!" : "MISS ((";

        await _telegramBotClient.EditMessageTextAsync(_chatId, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}