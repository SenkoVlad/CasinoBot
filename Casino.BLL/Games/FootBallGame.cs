using Casino.BLL.ButtonsGenerators;
using Casino.Common.AppConstants;
using Casino.DAL.Repositories.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Casino.BLL.Games;

public class FootBallGame : Game
{
    public readonly Message _message;
    private readonly ITelegramBotClient _telegramBotClient;
    public readonly IBalanceRepository _balanceRepository;
    private int? _scoreResult;
    private int _goodLuckMessageId;
    private bool _didWin;
    public bool IsWon => _didWin;
    public FootBallGame(Message message,
        ITelegramBotClient telegramBotClient,
        IBalanceRepository balanceRepository) : base(message, balanceRepository)
    {
        _message = message;
        _telegramBotClient = telegramBotClient;
        _balanceRepository = balanceRepository;
    }

    protected override async Task InitGameAsync()
    {
        var inlineKeyboardButtonsGenerator = new InlineKeyboardButtonsGenerator();
        inlineKeyboardButtonsGenerator.InitPlayFootballDemoButtons();
        var inlineKeyboardButtons = inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        var balance = _balanceRepository.GetBalanceAsync(_message.Chat.Id);
        var footballGameButtonText = $"{ButtonTextConstants.FootballGameButtonText}. Your balance: {balance}";

        await _telegramBotClient.SendTextMessageAsync(_message.Chat.Id, text: footballGameButtonText,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override async Task SentStartMessageAsync()
    {
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId,
            MessageTextConstants.GoodLuckFootBallMessageText);
        _goodLuckMessageId = goodLuckMessage.MessageId;
    }

    protected override async Task PlayGameRoundAsync()
    {
        var hitResult = await _telegramBotClient.SendDiceAsync(_message.Chat.Id, Emoji.Football);
        _scoreResult = hitResult.Dice?.Value;
    }

    protected override void SetRoundResult()
    {
        _didWin = DidYouWin();
    }

    protected override async Task SendRoundResultMessageAsync()
    {
        var roundResultMessage = _didWin ? "GOAL!" : "MISS ((";

        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }

    private bool DidYouWin()
    {
        return ReplyConstants.GoalScores.Contains((int)_scoreResult!);
    }
}