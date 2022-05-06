using Casino.BLL.ButtonsGenerators;
using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;
using Casino.Common.AppConstants;
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
    private readonly GameModel _gameModel;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IChatService _chatService;
    private int? _scoreResult;
    private int _goodLuckMessageId;

    private static readonly int[] GoalScores = { 5, 3, 4 };

    public FootBallGame(
        GameModel gameModel,
        int messageId,
        ITelegramBotClient telegramBotClient,
        IServiceProvider serviceProvider) : base(gameModel, serviceProvider)
    {
        _gameModel = gameModel;
        _telegramBotClient = telegramBotClient;
        _chatService = serviceProvider.GetRequiredService<IChatService>();
        _inlineKeyboardButtonsGenerator = serviceProvider.GetRequiredService<InlineKeyboardButtonsGenerator>();
        _localizer = serviceProvider.GetRequiredService<IStringLocalizer<Resources>>();
        _messageId = messageId;
    }

    protected override async Task SendDoNotHaveEnoughMoneyToPlayMessageAsync()
    {
        var message = _gameModel.IsDemoPlay ?
                _localizer[Resources.GetMyDemoBalanceResource, _gameModel.Chat.DemoBalance] :
                _localizer[Resources.DoNotHaveEnoughMoneyToRealPlayResource];
        await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, _messageId, message,
            replyMarkup: _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup);
    }

    protected override async Task InitGameAsync()
    {
        var chat = await _chatService.GetChatByIdOrException(_gameModel.Chat.Id);
        _gameModel.Chat = new ChatModel
        {
            Id = chat.Id,
            Balance = chat.Balance,
            DemoBalance = chat.DemoBalance
        };
        _inlineKeyboardButtonsGenerator.InitPlayFootballButtons(_gameModel);
        var inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        var footballGameButtonText = _gameModel.IsDemoPlay
            ? _localizer[Resources.GetMyDemoBalanceResource, _gameModel.Chat.DemoBalance]
            : _localizer[Resources.GetMyBalanceResource, _gameModel.Chat.Balance];
        await _telegramBotClient.SendTextMessageAsync(_gameModel.Chat.Id, text: footballGameButtonText,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override async Task SentStartMessageAsync()
    {
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, _messageId,
            _localizer[Resources.GoodLuckResource]);
        _goodLuckMessageId = goodLuckMessage.MessageId;
    }

    protected override async Task PlayGameRoundAsync()
    {
        var hitResult = await _telegramBotClient.SendDiceAsync(_gameModel.Chat.Id, Emoji.Football);
        _scoreResult = hitResult.Dice?.Value;
    }

    protected override bool GetRoundResult()
    {
        return GoalScores.Contains((int)_scoreResult!);
    }

    protected override async Task SendRoundResultMessageAsync()
    {
        var roundResultMessage = _gameModel.DidWin 
            ? _localizer[Resources.FootballGoalResource] 
            : _localizer[Resources.FootballMissResource];
        await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}