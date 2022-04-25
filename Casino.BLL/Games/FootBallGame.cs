using Casino.BLL.ButtonsGenerators;
using Casino.BLL.ClickHandlers.Implementation;
using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;
using Casino.Common.AppConstants;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Casino.BLL.Games;

public class FootBallGame : Game
{
    private readonly int _messageId;
    private readonly InlineKeyboardButtonsGenerator _inlineKeyboardButtonsGenerator;
    private readonly IStringLocalizer<Resources> _localizer;
    private readonly IBalanceRepository _balanceRepository;
    private readonly GameModel _gameModel;
    private readonly ITelegramBotClient _telegramBotClient;
    private int? _scoreResult;
    private int _goodLuckMessageId;

    private static readonly int[] GoalScores = { 5, 3, 4 };

    public FootBallGame(
        GameModel gameModel,
        int messageId,
        ITelegramBotClient telegramBotClient,
        IChatService chatService, 
        IServiceProvider serviceProvider) : base(gameModel, chatService)
    {
        _gameModel = gameModel;
        _telegramBotClient = telegramBotClient;
        _inlineKeyboardButtonsGenerator = serviceProvider.GetRequiredService<InlineKeyboardButtonsGenerator>();
        _localizer = serviceProvider.GetRequiredService<IStringLocalizer<Resources>>();
        _balanceRepository = serviceProvider.GetRequiredService<IBalanceRepository>();
        _messageId = messageId;
    }

    protected override async Task InitDemoGameAsync()
    {
        _inlineKeyboardButtonsGenerator.InitPlayFootballDemoButtons(_gameModel.UserBet);
        var inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        var balance = _balanceRepository.GetBalanceAsync(_gameModel.ChatId);
        var footballGameButtonText = $"{_localizer[Resources.GoodLuckFootBallMessageText]}. Your balance: {balance}";
        await _telegramBotClient.SendTextMessageAsync(_gameModel.ChatId, text: footballGameButtonText,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override Task InitRealGameAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task SentStartMessageAsync()
    {
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_gameModel.ChatId, _messageId,
            _localizer[Resources.GoodLuckFootBallMessageText]);
        _goodLuckMessageId = goodLuckMessage.MessageId;
    }

    protected override async Task PlayGameRoundAsync()
    {
        var hitResult = await _telegramBotClient.SendDiceAsync(_gameModel.ChatId, Emoji.Football);
        _scoreResult = hitResult.Dice?.Value;
    }

    protected override bool GetRoundResult()
    {
        return GoalScores.Contains((int)_scoreResult!);
    }

    protected override async Task SendRoundResultMessageAsync()
    {
        var roundResultMessage = _gameModel.DidWin ? "GOAL!" : "MISS ((";

        await _telegramBotClient.EditMessageTextAsync(_gameModel.ChatId, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}