using Casino.BLL.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ScreenHandlers.Interfaces;

public interface IScreenHandler
{
    public Task PushButtonAsync(CommandModel commandText);
    public string? GetReplyText { get; }
    public ReplyKeyboardMarkup? GetReplyKeyboardButtons { get; }
}