using Casino.BLL.ButtonsGenerators;
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
            case ButtonTextConstants.BackFromFootballButtonCallbackData:
                await PushGamesButtonAsync();
                break;
            case ButtonTextConstants.MenuButtonText:
                await PushMenuButtonAsync();
                break;
            case ButtonTextConstants.BalanceButtonText:
                await PushGetBalanceButtonAsync();
                break;
            case ButtonTextConstants.FootballGameButtonText:
                await PushFootballButtonAsync();
                break;
            case ButtonTextConstants.HitBallButtonText:
                await PushHitBallButtonAsync();
                break;
            case ButtonTextConstants.DiceGameButtonText:
                await PushDiceButtonAsync();
                break;
        }
    }

    private async Task PushDiceButtonAsync()
    {
        throw new Exception();
    }

    private async Task PushHitBallButtonAsync()
    {
        await _telegramBotClient.DeleteMessageAsync(_message.Chat.Id, _message.MessageId, _cancellationToken);

        var goodLuckMessage = await _telegramBotClient.SendTextMessageAsync(_message.Chat.Id, MessageTextConstants.GoodLuckFootBallMessageText,
            cancellationToken: _cancellationToken);
        var goodLuckMessageId = goodLuckMessage.MessageId;

        var hitResult = await _telegramBotClient.SendDiceAsync(_message.Chat.Id, Emoji.Football,
            cancellationToken: _cancellationToken);
        var score = hitResult.Dice?.Value;
        await Task.Delay(3500, _cancellationToken);

        _replyText = IsItGoal(score) ? "GOAL!" : "MISS ((";
        
        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, text: _replyText, messageId: goodLuckMessageId, cancellationToken: _cancellationToken);
        _inlineKeyboardButtonsGenerator.InitFootballButtons();
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = ButtonTextConstants.FootballGameButtonText;
        
        await _telegramBotClient.SendTextMessageAsync(_message.Chat.Id, text: _replyText, 
            replyMarkup: _inlineKeyboardButtons, cancellationToken: _cancellationToken);
    }

    private static bool IsItGoal(int? score)
    {
        return ReplyConstants.GoalScores.Contains((int) score!);
    }

    private async Task PushFootballButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitFootballButtons();
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = ButtonTextConstants.FootballGameButtonText;
     
        await _telegramBotClient.EditMessageReplyMarkupAsync(_message.Chat.Id, _message.MessageId,
            _inlineKeyboardButtons, _cancellationToken);
    }

    private async Task PushGetBalanceButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitGetBalanceButtons();
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = _balanceRepository.GetBalanceAsync(_message.Chat.Id).ToString();
        
        await _telegramBotClient.EditMessageTextAsync(_message.Chat.Id, _message.MessageId, 
            text: _replyText, cancellationToken: _cancellationToken);
        await _telegramBotClient.EditMessageReplyMarkupAsync(_message.Chat.Id, _message.MessageId,
            _inlineKeyboardButtons, _cancellationToken);
    }

    private async Task PushMenuButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitStartButtons();
        _replyText = ButtonTextConstants.GamesButtonText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
       
        await _telegramBotClient.EditMessageReplyMarkupAsync(_message.Chat.Id, _message.MessageId,
            _inlineKeyboardButtons, _cancellationToken);
    }

    private async Task PushGamesButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitGamesButtons();
        _replyText = ButtonTextConstants.GamesButtonText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
    
        await _telegramBotClient.EditMessageReplyMarkupAsync(_message.Chat.Id, _message.MessageId,
            _inlineKeyboardButtons, _cancellationToken);
    }
}