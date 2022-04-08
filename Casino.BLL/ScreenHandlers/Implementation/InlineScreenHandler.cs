using Casino.BLL.ScreenHandlers.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ScreenHandlers.Implementation;

public class InlineScreenHandler : IScreenHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _cancellationToken;

    public string GetReplyText => "";
    public ReplyKeyboardMarkup GetReplyKeyboardButtons => null!;

    public InlineScreenHandler(ITelegramBotClient telegramBotClient, CancellationToken cancellationToken)
    {
        _telegramBotClient = telegramBotClient;
        _cancellationToken = cancellationToken;
    }

    public Task PushButtonAsync(string? commandText)
    {
        return Task.CompletedTask;
    }


}