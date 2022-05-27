using Casino.BLL.ButtonsGenerators;
using Casino.BLL.Models;
using Casino.Common.AppConstants;
using Casino.Common.Enum;
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
            Localizer[Resources.DoNotHaveEnoughMoneyToDemoPlayResource] :
            Localizer[Resources.DoNotHaveEnoughMoneyToRealPlayResource];
        await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, _messageId, message,
            replyMarkup: _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup);
    }

    protected override async Task InitGameAsync()
    {
        var chatModel = await ChatService.GetChatByIdOrException(_gameModel.Chat.Id);
        _gameModel.Chat = chatModel;
        var initGameModel = new InitGameModel
        {
            PlayButtonText = Localizer[Resources.ThrowDartButtonText],
            ChangeGameBetCommand = Command.ChangeDartsBet,
            PlayGameCommand = Command.ThrowDarts,
            PlayToCommand = Command.PlayDarts
        };
        _inlineKeyboardButtonsGenerator.InitPlayGameButtons(_gameModel, initGameModel);
        var inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        var footballGameButtonText = _gameModel.IsDemoPlay
            ? Localizer[Resources.GetMyDemoBalanceResource, _gameModel.Chat.DemoBalance]
            : Localizer[Resources.GetMyBalanceResource, _gameModel.Chat.Balance];
        await _telegramBotClient.SendTextMessageAsync(_gameModel.Chat.Id, text: footballGameButtonText,
            replyMarkup: inlineKeyboardButtons);
    }

    protected override async Task SentStartMessageAsync()
    {
        var startMessage = $"{Localizer[Resources.GoodLuckResource]}";
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
                ? Localizer[Resources.DartsWonResource, (int)WinningsScore, _gameModel.IsDemoPlay ? AppConstants.DemoCurrencySign : AppConstants.RealCurrencySign]
                : Localizer[Resources.DartsLostResource];
        }
        else
        {
            roundResultMessage = saveGameResultModel.Message;
        }
        await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}