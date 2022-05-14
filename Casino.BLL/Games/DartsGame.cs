using Casino.BLL.ButtonsGenerators;
using Casino.BLL.Models;
using Casino.Common.AppConstants;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Casino.BLL.Games;

class DartsGame : Game
{
    private readonly GameModel _gameModel;
    private readonly int _messageId;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly InlineKeyboardButtonsGenerator _inlineKeyboardButtonsGenerator;
    private int _goodLuckMessageId;

    public DartsGame(GameModel gameModel,
        int messageId, 
        ITelegramBotClient telegramBotClient,
        IServiceProvider serviceProvider) : base(gameModel, serviceProvider, AppConstants.DartsDelayAfterRound)
    {
        _gameModel = gameModel;
        _messageId = messageId;
        _telegramBotClient = telegramBotClient;
        _inlineKeyboardButtonsGenerator = serviceProvider.GetRequiredService<InlineKeyboardButtonsGenerator>();
    }

    protected override async Task SendDoNotHaveEnoughMoneyToPlayMessageAsync()
    {
        var message = _gameModel.IsDemoPlay ?
            _localizer[Resources.DoNotHaveEnoughMoneyToDemoPlayResource] :
            _localizer[Resources.DoNotHaveEnoughMoneyToRealPlayResource];
        await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, _messageId, message,
            replyMarkup: _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup);
    }

    protected override async Task InitGameAsync()
    {
        var chatModel = await _chatService.GetChatByIdOrException(_gameModel.Chat.Id);
        _gameModel.Chat = chatModel;
        _inlineKeyboardButtonsGenerator.InitDartsChooseBetButtons(_gameModel);
        var inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        var footballGameButtonText = _gameModel.IsDemoPlay
            ? _localizer[Resources.GetMyDemoBalanceResource, _gameModel.Chat.DemoBalance]
            : _localizer[Resources.GetMyBalanceResource, _gameModel.Chat.Balance];
        await _telegramBotClient.SendTextMessageAsync(_gameModel.Chat.Id, text: footballGameButtonText,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override async Task SentStartMessageAsync()
    {
        var startMessage = $"{_localizer[Resources.GoodLuckResource]}";
        var goodLuckMessage = await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, _messageId, startMessage);
        _goodLuckMessageId = goodLuckMessage.MessageId;
    }

    protected override async Task PlayGameRoundAsync()
    {
        var dartsResult = await _telegramBotClient.SendDiceAsync(_gameModel.Chat.Id, Emoji.Darts);
        _gameModel.DiceResult = dartsResult.Dice!.Value;
    }

    protected override async Task SendRoundResultMessageAsync(SaveGameResultModel saveGameResultModel)
    {
        string roundResultMessage;
        if (saveGameResultModel.Success)
        {
            roundResultMessage = _gameModel.BettingResult.IsWon
                ? _localizer[Resources.DartsWonResource, (int)WinningsScore, _gameModel.IsDemoPlay ? AppConstants.DemoCurrencySign : AppConstants.RealCurrencySign]
                : _localizer[Resources.DartsLostResource];
        }
        else
        {
            roundResultMessage = saveGameResultModel.Message;
        }
        await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}