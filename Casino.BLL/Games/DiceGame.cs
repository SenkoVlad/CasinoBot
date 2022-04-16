using Casino.BLL.ButtonsGenerators;
using Casino.Common.AppConstants;
using Casino.DAL.Repositories.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Casino.BLL.Games;

public class DiceGame : Game
{
    private readonly Message _message;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IBalanceRepository _balanceRepository;
    private readonly int _diceBet;
    private int? _scoreResult;
    private int _goodLuckMessageId;
    private bool _didWin;
    public bool IsWon => _didWin;

    public DiceGame(Message message, 
        ITelegramBotClient telegramBotClient,
        IBalanceRepository balanceRepository,
        int diceBet) : base(message, balanceRepository)
    {
        _message = message;
        _telegramBotClient = telegramBotClient;
        _balanceRepository = balanceRepository;
        _diceBet = diceBet;
    }

    protected override async Task InitGameAsync()
    {
        var inlineKeyboardButtonsGenerator = new InlineKeyboardButtonsGenerator();
        inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons();
        var balance = _balanceRepository.GetBalanceAsync(_message.Chat.Id);
        var chooseYourBetMessage = $"{MessageTextConstants.ChooseYourBetMessageText}. Your balance: {balance}";
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

    protected override void SetRoundResult()
    {
        _didWin = _scoreResult == _diceBet;
    }

    protected override async Task SendRoundResultMessageAsync()
    {
        var roundResultMessage = _didWin ? "Win" : "Lose";
        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}