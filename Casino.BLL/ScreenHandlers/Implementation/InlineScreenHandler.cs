using System.Linq;
using Casino.BLL.ButtonsGenerators;
using Casino.BLL.Games;
using Casino.BLL.Models;
using Casino.BLL.ScreenHandlers.Interfaces;
using Casino.Common.AppConstants;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ScreenHandlers.Implementation;

public class InlineScreenHandler : IScreenHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _cancellationToken;
    private readonly IBalanceRepository _balanceRepository;
    private readonly Message _message;
    private readonly InlineKeyboardButtonsGenerator _inlineKeyboardButtonsGenerator;
    private InlineKeyboardMarkup? _inlineKeyboardButtons;
    private string? _replyText;

    public string? GetReplyText => _replyText;
    public ReplyKeyboardMarkup GetReplyKeyboardButtons => null!;

    public InlineScreenHandler(Message message,
        ITelegramBotClient telegramBotClient, 
        CancellationToken cancellationToken,
        IServiceProvider serviceProvider)
    {
        var text = message.Text;
        _message = message;
        _telegramBotClient = telegramBotClient;
        _cancellationToken = cancellationToken;
        _balanceRepository = serviceProvider.GetRequiredService<IBalanceRepository>();
        _inlineKeyboardButtonsGenerator = new InlineKeyboardButtonsGenerator();
    }

    public async Task PushButtonAsync(CommandModel commandModel)
    {
        var commandText = commandModel.CommandText;
        switch (commandText)
        {
            case ButtonTextConstants.GamesButtonText:
            case ButtonTextConstants.BackFromFootballButtonText:
            case ButtonTextConstants.BackFromDiceBetButtonText:
                await PushGamesButtonAsync();
                break;
            case AppConstants.StartCommand:
                await StartBotAsync();
                break;
            case ButtonTextConstants.MenuButtonText:
                await PushMenuButtonAsync();
                break;
            case ButtonTextConstants.BalanceButtonText:
                await PushGetBalanceButtonAsync();
                break;
            case ButtonTextConstants.FootballGameButtonText:
                await PushChooseFootballGameButtonAsync();
                break;
            case ButtonTextConstants.HitBallButtonText:
                await PushHitBallButtonAsync();
                break;
            case ButtonTextConstants.DiceGameButtonText:
                await PushDiceButtonAsync();
                break;
            case ButtonTextConstants.DicePointChosenButtonText:
                var dice = commandModel.CommandParam;
                await PushChooseDice(dice);
                break;
        }
    }

    private async Task PushChooseDice(int diceBet)
    {
        var diceGame = new DiceGame(_message, _telegramBotClient, diceBet);
        await diceGame.PlayRoundAsync();
    }

    private async Task StartBotAsync()
    {
        _inlineKeyboardButtonsGenerator.InitStartButtons();
        _replyText = ButtonTextConstants.GamesButtonText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.SendTextMessageAsync(_message.Chat.Id, MessageTextConstants.StartMessageText,
            replyMarkup: _inlineKeyboardButtons, cancellationToken: _cancellationToken);
    }

    private async Task PushDiceButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons();
        _replyText = MessageTextConstants.ChooseYourBet;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId, _replyText,
            replyMarkup: _inlineKeyboardButtons,cancellationToken: _cancellationToken);
    } 

    private async Task PushHitBallButtonAsync()
    {
        var footBallGame = new FootBallGame(_message, _telegramBotClient);
        await footBallGame.PlayRoundAsync();
    }

    private void InitFootBallGame()
    {
        _inlineKeyboardButtonsGenerator.InitFootballButtons();
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = ButtonTextConstants.FootballGameButtonText;
    }

    private async Task PushChooseFootballGameButtonAsync()
    {
        InitFootBallGame();
        await _telegramBotClient.EditMessageReplyMarkupAsync(_message.Chat.Id, _message.MessageId,
            _inlineKeyboardButtons, _cancellationToken);
    }

    private async Task PushGetBalanceButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitGetBalanceButtons();
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = _balanceRepository.GetBalanceAsync(_message.Chat.Id).ToString();
        
        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId, 
            _replyText, replyMarkup: _inlineKeyboardButtons, cancellationToken: _cancellationToken);
    }

    private async Task PushMenuButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitStartButtons();
        _replyText = ButtonTextConstants.GamesButtonText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId,
            MessageTextConstants.StartMessageText, replyMarkup: _inlineKeyboardButtons, cancellationToken: _cancellationToken);
    }

    private async Task PushGamesButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitGamesButtons();
        _replyText = ButtonTextConstants.GamesButtonText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId,
            MessageTextConstants.StartMessageText, cancellationToken: _cancellationToken, replyMarkup: _inlineKeyboardButtons);
    }
}