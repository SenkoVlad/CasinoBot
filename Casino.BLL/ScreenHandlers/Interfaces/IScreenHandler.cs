using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ScreenHandlers.Interfaces;

public interface IScreenHandler
{
    public Task PushButtonAsync(string? commandText);
    public string? GetReplyText { get; }
    public ReplyKeyboardMarkup? GetReplyKeyboardButtons { get; }
}