using Casino.BLL.ButtonsGenerators;
using Casino.BLL.ClickHandlers.Implementation;
using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;
using Casino.Common.AppConstants;
using Casino.DAL.Models;
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
        IChatService chatService, 
        IServiceProvider serviceProvider) : base(gameModel, chatService)
    {
        _gameModel = gameModel;
        _telegramBotClient = telegramBotClient;
        _chatService = chatService;
        _inlineKeyboardButtonsGenerator = serviceProvider.GetRequiredService<InlineKeyboardButtonsGenerator>();
        _localizer = serviceProvider.GetRequiredService<IStringLocalizer<Resources>>();
        _messageId = messageId;
    }

    protected override async Task InitDemoGameAsync()
    {
        var chat = await _chatService.GetChatByIdOrException(_gameModel.Chat.Id);
        _gameModel.Chat = new ChatModel
        {
            Id = chat.Id,
            Balance = chat.Balance,
            DemoBalance = chat.DemoBalance
        };
        _inlineKeyboardButtonsGenerator.InitPlayDemoFootballButtons(_gameModel);
        var inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        var footballGameButtonText = _localizer[Resources.GetMyDemoBalanceMessageText, _gameModel.Chat.DemoBalance];
        await _telegramBotClient.SendTextMessageAsync(_gameModel.Chat.Id, text: footballGameButtonText,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override Task InitRealGameAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task SentStartMessageAsync()
    {
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, _messageId,
            _localizer[Resources.GoodLuckFootBallMessageText]);
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
        var roundResultMessage = _gameModel.DidWin ? "GOAL!" : "MISS ((";

        await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}