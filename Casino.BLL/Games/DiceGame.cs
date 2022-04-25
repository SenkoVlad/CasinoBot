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
    private readonly IBalanceRepository _balanceRepository;
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
        _diceBet = diceBet;
        _inlineKeyboardButtonsGenerator = serviceProvider.GetRequiredService<InlineKeyboardButtonsGenerator>();
        _localizer = serviceProvider.GetRequiredService<IStringLocalizer<Resources>>();
        _balanceRepository = serviceProvider.GetRequiredService<IBalanceRepository>();
    }

    protected override async Task InitDemoGameAsync()
    {
        _inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons(_gameModel.UserBet);
        var balance = _balanceRepository.GetBalanceAsync(_gameModel.ChatId);
        var chooseYourBetMessage = $"{_localizer[Resources.ChooseYourBetMessageText]}. Your balance: {balance}";
        var inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.SendTextMessageAsync(_gameModel.ChatId, text: chooseYourBetMessage,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override Task InitRealGameAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task SentStartMessageAsync()
    {
        var startMessage = $"{_localizer[Resources.GoodLuckFootBallMessageText]}. Your bet is {_diceBet}";
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_gameModel.ChatId, _messageId, startMessage);
        _goodLuckMessageId = goodLuckMessage.MessageId;
    }

    protected override async Task PlayGameRoundAsync()
    {
        var diceResult = await _telegramBotClient.SendDiceAsync(_gameModel.ChatId, Emoji.Dice);
        _scoreResult = diceResult.Dice?.Value;
    }

    protected override bool GetRoundResult()
    {
        return _scoreResult == _diceBet;
    }

    protected override async Task SendRoundResultMessageAsync()
    {
        var roundResultMessage = _gameModel.DidWin ? "Win" : "Lose";
        await _telegramBotClient.EditMessageTextAsync(_gameModel.ChatId, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}