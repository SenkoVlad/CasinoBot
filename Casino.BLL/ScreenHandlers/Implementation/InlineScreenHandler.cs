using Casino.BLL.ButtonsGenerators;
using Casino.BLL.Games;
using Casino.BLL.ScreenHandlers.Interfaces;
using Casino.Common.AppConstants;
using Casino.Common.Dtos;
using Casino.Common.Enum;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ScreenHandlers.Implementation;

public class InlineScreenHandler : IScreenHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IBalanceRepository _balanceRepository;
    private readonly long _chatId;
    private readonly int _messageId;
    private readonly InlineKeyboardButtonsGenerator _inlineKeyboardButtonsGenerator;
    private InlineKeyboardMarkup? _inlineKeyboardButtons;
    private string? _replyText;

    public string? GetReplyText => _replyText;
    public ReplyKeyboardMarkup GetReplyKeyboardButtons => null!;

    public InlineScreenHandler(long chatId,
        int messageId,
        ITelegramBotClient telegramBotClient, 
        IServiceProvider serviceProvider)
    {
        _chatId = chatId;
        _telegramBotClient = telegramBotClient;
        _messageId = messageId;
        _balanceRepository = serviceProvider.GetRequiredService<IBalanceRepository>();
        _inlineKeyboardButtonsGenerator = new InlineKeyboardButtonsGenerator();
    }

    public async Task PushButtonAsync(CommandDto commandDto)
    {
        var command = commandDto.Command;
        switch (command)
        {
            case Command.GetGamesButtonCommand:
                await PushGamesButtonAsync();
                break;
            case Command.StartCommand:
                await StartBotAsync();
                break;
            case Command.ToMenuButtonCommand:
                await PushMenuButtonAsync();
                break;
            case Command.GetBalanceButtonCommand:
                await PushGetBalanceButtonAsync();
                break;
            case Command.ChooseFootballGameButtonCommand:
                await PushChooseFootballGame();
                break;
            case Command.HitBallButtonCommand:
                await PushHitBallButtonAsync();
                break;
            case Command.ChooseDiceGameButtonCommand:
                await PushDiceButtonAsync();
                break;
            case Command.DemoPlayFootballButtonCommand:
                await PushFootballDemoPlayButtonAsync();
                break;
            case Command.MakeBetButtonCommand:
                var dice = commandDto.CommandParam;
                await PushChooseDice(dice);
                break;
        }
    }

    private async Task PushChooseFootballGame()
    {
        _inlineKeyboardButtonsGenerator.InitChooseFootballMode();
        _replyText = MessageTextConstants.ChooseGameModeMessageText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.EditMessageTextAsync(_chatId, _messageId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushChooseDice(int diceBet)
    {
        var diceGame = new DiceGame(_chatId, _messageId, _telegramBotClient, _balanceRepository, diceBet);
        await diceGame.PlayRoundAsync();
    }

    private async Task StartBotAsync()
    {
        _inlineKeyboardButtonsGenerator.InitStartButtons();
        _replyText = MessageTextConstants.ChooseActionMessageText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.SendTextMessageAsync(_chatId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushDiceButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons();
        _replyText = MessageTextConstants.ChooseYourBetMessageText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.EditMessageTextAsync(_chatId, _messageId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    } 

    private async Task PushHitBallButtonAsync()
    {
        var footBallGame = new FootBallGame(_chatId, _messageId, _telegramBotClient, _balanceRepository);
        await footBallGame.PlayRoundAsync();
    }

    private async Task PushFootballDemoPlayButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitPlayFootballDemoButtons();
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = $"{ButtonTextConstants.FootballGameButtonText}"; await _telegramBotClient.EditMessageTextAsync(_chatId, _messageId, 
            text: _replyText,replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushGetBalanceButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitGetBalanceButtons();
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = _balanceRepository.GetBalanceAsync(_chatId).ToString();
        
        await _telegramBotClient.EditMessageTextAsync(_chatId, _messageId, 
            _replyText, replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushMenuButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitStartButtons();
        _replyText = MessageTextConstants.ChooseActionMessageText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.EditMessageTextAsync(_chatId, _messageId,
            MessageTextConstants.ChooseActionMessageText, replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushGamesButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitGamesButtons();
        _replyText = MessageTextConstants.ChooseActionMessageText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.EditMessageTextAsync(_chatId, _messageId,
            MessageTextConstants.ChooseActionMessageText, replyMarkup: _inlineKeyboardButtons);
    }
}