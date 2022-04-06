using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ButtonPushHandlers.Interfaces;

public interface IButtonPushHandler
{
    public Task PushAsync(string? commandText);
    public string? GetReplyText();
    public ReplyKeyboardMarkup? GetReplyButtons();
}