using Casino.BLL.ButtonsGenerators;
using Casino.BLL.ClickHandlers.Implementation;
using Casino.Common.AppConstants;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Casino.BLL.Games;

public class DiceGame : Game
{
    private readonly long _chatId;
    private readonly int _messageId;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IBalanceRepository _balanceRepository;
    private readonly int _diceBet;
    private readonly InlineKeyboardButtonsGenerator _inlineKeyboardButtonsGenerator;
    private readonly IStringLocalizer<Resources> _localizer;
    private int? _scoreResult;
    private int _goodLuckMessageId;
    
    public DiceGame(long chatId,
        int messageId,
        ITelegramBotClient telegramBotClient,
        IBalanceRepository balanceRepository,
        int diceBet,
        int userBet,
        InlineKeyboardButtonsGenerator inlineKeyboardButtonsGenerator,
        IStringLocalizer<Resources> localizer) : base(chatId, balanceRepository, userBet)
    {
        _chatId = chatId;
        _telegramBotClient = telegramBotClient;
        _balanceRepository = balanceRepository;
        _diceBet = diceBet;
        _inlineKeyboardButtonsGenerator = inlineKeyboardButtonsGenerator;
        _localizer = localizer;
        _messageId = messageId;
    }

    protected override async Task InitGameAsync()
    {
        _inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons(UserBet);
        var balance = _balanceRepository.GetBalanceAsync(_chatId);
        var chooseYourBetMessage = $"{_localizer[Resources.ChooseYourBetMessageText]}. Your balance: {balance}";
        var inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.SendTextMessageAsync(_chatId, text: chooseYourBetMessage,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override async Task SentStartMessageAsync()
    {
        var startMessage = $"{_localizer[Resources.GoodLuckFootBallMessageText]}. Your bet is {_diceBet}";
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_chatId, _messageId, startMessage);
        _goodLuckMessageId = goodLuckMessage.MessageId;
    }

    protected override async Task PlayGameRoundAsync()
    {
        var diceResult = await _telegramBotClient.SendDiceAsync(_chatId, Emoji.Dice);
        _scoreResult = diceResult.Dice?.Value;
    }

    protected override bool GetRoundResult()
    {
        return _scoreResult == _diceBet;
    }

    protected override async Task SendRoundResultMessageAsync()
    {
        var roundResultMessage = DidWin ? "Win" : "Lose";
        await _telegramBotClient.EditMessageTextAsync(_chatId, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}