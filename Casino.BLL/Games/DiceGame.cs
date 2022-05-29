using Casino.BLL.ButtonsGenerators;
using Casino.BLL.Models;
using Casino.Common.AppConstants;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Casino.BLL.Games;

public class DiceGame : Game
{
    private readonly GameModel _gameModel;
    private readonly int _messageId;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly int _diceBet;
    private readonly InlineKeyboardButtonsGenerator _inlineKeyboardButtonsGenerator;
    private int _goodLuckMessageId;
    private int? _scoreResult;

    public DiceGame(
        GameModel gameModel,
        int messageId,
        ITelegramBotClient telegramBotClient,
        int diceBet,
        IServiceProvider serviceProvider) : base(gameModel, serviceProvider, AppConstants.DiceDelayAfterRound)
    {
        _gameModel = gameModel;
        _messageId = messageId;
        _telegramBotClient = telegramBotClient;
        _diceBet = diceBet;
        _inlineKeyboardButtonsGenerator = serviceProvider.GetRequiredService<InlineKeyboardButtonsGenerator>();
    }

    protected override async Task SendDoNotHaveEnoughMoneyToPlayMessageAsync()
    {
        var message = _gameModel.IsDemoPlay ?
            Localizer[Resources.DoNotHaveEnoughMoneyToDemoPlayResource]:
            Localizer[Resources.DoNotHaveEnoughMoneyToRealPlayResource];
        await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, _messageId, message,
            replyMarkup: _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup);
    }

    protected override async Task InitGameAsync()
    {
        var chatModel = await ChatService.GetChatByIdOrException(_gameModel.Chat.Id);
        _gameModel.Chat = chatModel;
        _inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons(_gameModel, _diceBet);
        var chooseYourBetMessage = _gameModel.IsDemoPlay
            ? Localizer[Resources.GetMyDemoBalanceResource, _gameModel.Chat.DemoBalance]
            : Localizer[Resources.GetMyBalanceResource, _gameModel.Chat.Balance];
        var inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.SendTextMessageAsync(_gameModel.Chat.Id, text: chooseYourBetMessage,
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
        var diceResult = await _telegramBotClient.SendDiceAsync(_gameModel.Chat.Id, Emoji.Dice);
        _scoreResult = diceResult.Dice?.Value;
    }

    protected override void SetRoundResult()
    {
        var bettingResult = GameParameters.BettingResults.FirstOrDefault(b => b.GameId == _gameModel.GameId &&
                                                                               b.IsWon == (_diceBet == _scoreResult));
        
        if (bettingResult == null)
        {
            throw new Exception($"{nameof(SetRoundResult)} bettingResult was not found");
        }

        _gameModel.BettingResult = new BettingResultModel
        {
            Id = bettingResult.Id,
            Coefficient = bettingResult.Coefficient,
            IsWon = bettingResult.IsWon
        };
    }

    protected override async Task SendRoundResultMessageAsync(SaveGameResultModel saveGameResultModel)
    {
        string roundResultMessage;
        if (saveGameResultModel.Success)
        {
            roundResultMessage = _gameModel.BettingResult.IsWon
                ? Localizer[Resources.DiceWonResource, (int)WinningsScore, _gameModel.IsDemoPlay ? AppConstants.DemoCurrencySign : AppConstants.RealCurrencySign]
                : Localizer[Resources.DiceLostResource];
        }
        else
        {
            roundResultMessage = saveGameResultModel.Message;
        }
        await _telegramBotClient.EditMessageTextAsync(_gameModel.Chat.Id, text: roundResultMessage,
            messageId: _goodLuckMessageId);
    }
}