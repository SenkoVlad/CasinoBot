using Casino.BLL.ButtonsGenerators;
using Casino.BLL.Games;
using Casino.BLL.Models;
using Casino.BLL.ScreenHandlers.Interfaces;
using Casino.Common.AppConstants;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
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
            case ButtonCommands.GetGamesButtonCommand:
                await PushGamesButtonAsync();
                break;
            case AppConstants.StartCommand:
                await StartBotAsync();
                break;
            case ButtonCommands.ToMenuButtonCommand:
                await PushMenuButtonAsync();
                break;
            case ButtonCommands.GetBalanceButtonCommand:
                await PushGetBalanceButtonAsync();
                break;
            case ButtonCommands.ChooseFootballGameButtonCommand:
                await PushChooseFootballGame();
                break;
            case ButtonCommands.HitBallButtonCommand:
                await PushHitBallButtonAsync();
                break;
            case ButtonCommands.ChooseDiceGameButtonCommand:
                await PushDiceButtonAsync();
                break;
            case ButtonCommands.DemoPlayFootballButtonCommand:
                await PushFootballDemoPlayButtonAsync();
                break;
            case ButtonCommands.MakeBetButtonCommand:
                var dice = commandModel.CommandParam;
                await PushChooseDice(dice);
                break;
        }
    }

    private async Task PushChooseFootballGame()
    {
        _inlineKeyboardButtonsGenerator.InitChooseFootballMode();
        _replyText = MessageTextConstants.ChooseGameModeMessageText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId, _replyText,
            replyMarkup: _inlineKeyboardButtons, cancellationToken: _cancellationToken);
    }

    private async Task PushChooseDice(int diceBet)
    {
        var diceGame = new DiceGame(_message, _telegramBotClient, _balanceRepository, diceBet);
        await diceGame.PlayRoundAsync();
    }

    private async Task StartBotAsync()
    {
        _inlineKeyboardButtonsGenerator.InitStartButtons();
        _replyText = MessageTextConstants.ChooseActionMessageText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.SendTextMessageAsync(_message.Chat.Id, _replyText,
            replyMarkup: _inlineKeyboardButtons, cancellationToken: _cancellationToken);
    }

    private async Task PushDiceButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons();
        _replyText = MessageTextConstants.ChooseYourBetMessageText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId, _replyText,
            replyMarkup: _inlineKeyboardButtons,cancellationToken: _cancellationToken);
    } 

    private async Task PushHitBallButtonAsync()
    {
        var footBallGame = new FootBallGame(_message, _telegramBotClient, _balanceRepository);
        await footBallGame.PlayRoundAsync();
    }

    private async Task PushFootballDemoPlayButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitPlayFootballDemoButtons();
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = $"{ButtonTextConstants.FootballGameButtonText}"; await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId, text: _replyText,
            replyMarkup: _inlineKeyboardButtons, cancellationToken: _cancellationToken);
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
        _replyText = MessageTextConstants.ChooseActionMessageText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId,
            MessageTextConstants.ChooseActionMessageText, replyMarkup: _inlineKeyboardButtons, cancellationToken: _cancellationToken);
    }

    private async Task PushGamesButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitGamesButtons();
        _replyText = MessageTextConstants.ChooseActionMessageText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId,
            MessageTextConstants.ChooseActionMessageText, cancellationToken: _cancellationToken, replyMarkup: _inlineKeyboardButtons);
    }
}