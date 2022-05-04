using Casino.Common.Dtos;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ClickHandlers.Interfaces;

public interface IClickHandler
{
    public Task PushButtonAsync();
}