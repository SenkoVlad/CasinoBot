using Casino.BLL.ButtonPushHandlers.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ButtonPushHandlers.Implementation;

public class InlineButtonPushHandler : IButtonPushHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _cancellationToken;

    public InlineButtonPushHandler(ITelegramBotClient telegramBotClient, CancellationToken cancellationToken)
    {
        _telegramBotClient = telegramBotClient;
        _cancellationToken = cancellationToken;
    }

    public Task PushAsync(string? commandText)
    {
        return Task.CompletedTask;
    }

    public string GetReplyText()
    {
        return "";
    }

    public ReplyKeyboardMarkup GetReplyButtons()
    {
        return null!;
    }
}