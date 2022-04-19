using Casino.Common.Dtos;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ScreenHandlers.Interfaces;

public interface IScreenHandler
{
    public Task PushButtonAsync(CommandDto commandDto);
    public string? GetReplyText { get; }
    public ReplyKeyboardMarkup? GetReplyKeyboardButtons { get; }
}