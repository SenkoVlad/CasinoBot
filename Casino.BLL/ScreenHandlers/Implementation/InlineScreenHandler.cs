using Casino.BLL.ButtonsGenerators;
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

    public InlineScreenHandler(Message chatId,
        ITelegramBotClient telegramBotClient, 
        CancellationToken cancellationToken,
        IServiceProvider serviceProvider)
    {
        _message = chatId;
        _telegramBotClient = telegramBotClient;
        _cancellationToken = cancellationToken;
        _balanceRepository = serviceProvider.GetRequiredService<IBalanceRepository>(); ;
        _inlineKeyboardButtonsGenerator = new InlineKeyboardButtonsGenerator();
    }

    public async Task PushButtonAsync(string? commandText)
    {
        switch (commandText)
        {
            case ButtonTextConstants.GamesButtonText:
                PushGamesButton();
                break;
            case ButtonTextConstants.MenuButtonText:
                PushMenuButton();
                break;
            case ButtonTextConstants.BalanceButtonText:
                PushGetBalanceButton();
                break;
        }

        await _telegramBotClient.EditMessageReplyMarkupAsync(_message.Chat.Id, _message.MessageId,
            _inlineKeyboardButtons, _cancellationToken);
    }

    private async void PushGetBalanceButton()
    {
        _inlineKeyboardButtonsGenerator.InitGetBalanceButtons();
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = _balanceRepository.GetBalanceAsync(_message.Chat.Id).ToString();

        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId, 
            text: _replyText, cancellationToken: _cancellationToken);
    }

    private void PushMenuButton()
    {
        _inlineKeyboardButtonsGenerator.InitStartButtons();
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = ButtonTextConstants.GamesButtonText;
    }

    private void PushGamesButton()
    {
        _inlineKeyboardButtonsGenerator.InitGamesButtons();
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = ButtonTextConstants.GamesButtonText;
    }
}