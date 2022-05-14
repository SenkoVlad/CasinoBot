using Casino.BLL.ButtonsGenerators;
using Casino.BLL.Models;
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
    private int _goodLuckMessageId;

    public FootBallGame(
        GameModel gameModel,
        int messageId,
        ITelegramBotClient telegramBotClient,
        IServiceProvider serviceProvider) : base(gameModel, serviceProvider, AppConstants.FootballDelayAfterRound)
    {
        _gameModel = gameModel;
        _telegramBotClient = telegramBotClient;
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
        var chatModel = await _chatService.GetChatByIdOrException(_gameModel.Chat.Id);
        _gameModel.Chat = chatModel;
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
        _gameModel.DiceResult = hitResult.Dice!.Value;
    }

    protected override async Task SendRoundResultMessageAsync(SaveGameResultModel saveGameResultModel)
    {
        string roundResultMessage;
        if (saveGameResultModel.Success)
        {
            roundResultMessage = _gameModel.BettingResult.IsWon
                ? _localizer[Resources.FootballWonResource, (int)WinningsScore, _gameModel.IsDemoPlay ? AppConstants.DemoCurrencySign : AppConstants.RealCurrencySign]
                : _localizer[Resources.FootballMissResource];
        }
        else
        {
            roundResultMessage = saveGameResultModel.Message;
        }

        await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}