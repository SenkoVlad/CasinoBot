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

public class DiceGame : Game
{
    private readonly GameModel _gameModel;
    private readonly int _messageId;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IChatService _chatService;
    private readonly int _diceBet;
    private readonly InlineKeyboardButtonsGenerator _inlineKeyboardButtonsGenerator;
    private readonly IStringLocalizer<Resources> _localizer;
    private int? _scoreResult;
    private int _goodLuckMessageId;
    
    public DiceGame(
        GameModel gameModel,
        int messageId,
        ITelegramBotClient telegramBotClient,
        IChatService chatService,
        int diceBet,
        IServiceProvider serviceProvider) : base(gameModel, chatService)
    {
        _gameModel = gameModel;
        _messageId = messageId;
        _telegramBotClient = telegramBotClient;
        _chatService = chatService;
        _diceBet = diceBet;
        _inlineKeyboardButtonsGenerator = serviceProvider.GetRequiredService<InlineKeyboardButtonsGenerator>();
        _localizer = serviceProvider.GetRequiredService<IStringLocalizer<Resources>>();
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
        _inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons(_gameModel);
        var chooseYourBetMessage = _gameModel.IsDemoPlay
            ? _localizer[Resources.GetMyDemoBalanceMessageText, _gameModel.Chat.DemoBalance]
            : _localizer[Resources.GetMyBalanceMessageText, _gameModel.Chat.Balance];
        var inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.SendTextMessageAsync(_gameModel.Chat.Id, text: chooseYourBetMessage,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override async Task SentStartMessageAsync()
    {
        var startMessage = $"{_localizer[Resources.GoodLuckFootBallMessageText]}. Your bet is {_diceBet}";
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, _messageId, startMessage);
        _goodLuckMessageId = goodLuckMessage.MessageId;
    }

    protected override async Task PlayGameRoundAsync()
    {
        var diceResult = await _telegramBotClient.SendDiceAsync(_gameModel.Chat.Id, Emoji.Dice);
        _scoreResult = diceResult.Dice?.Value;
    }

    protected override bool GetRoundResult()
    {
        return _scoreResult == _diceBet;
    }

    protected override async Task SendRoundResultMessageAsync()
    {
        var roundResultMessage = _gameModel.DidWin ? "Win" : "Lose";
        await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}