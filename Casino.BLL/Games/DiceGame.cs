using Casino.BLL.ButtonsGenerators;
using Casino.Common.AppConstants;
using Casino.DAL.Repositories.Interfaces;
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
    private int? _scoreResult;
    private int _goodLuckMessageId;

    public DiceGame(long chatId,
        int messageId,
        ITelegramBotClient telegramBotClient,
        IBalanceRepository balanceRepository,
        int diceBet) : base(chatId, balanceRepository)
    {
        _chatId = chatId;
        _telegramBotClient = telegramBotClient;
        _balanceRepository = balanceRepository;
        _diceBet = diceBet;
        _messageId = messageId;
    }

    protected override async Task InitGameAsync()
    {
        var inlineKeyboardButtonsGenerator = new InlineKeyboardButtonsGenerator();
        inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons();
        var balance = _balanceRepository.GetBalanceAsync(_chatId);
        var chooseYourBetMessage = $"{MessageTextConstants.ChooseYourBetMessageText}. Your balance: {balance}";
        var inlineKeyboardButtons = inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.SendTextMessageAsync(_chatId, text: chooseYourBetMessage,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override async Task SentStartMessageAsync()
    {
        var startMessage = $"{MessageTextConstants.GoodLuckFootBallMessageText}. Your bet is {_diceBet}";
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