using Casino.DAL.Models;

namespace Casino.BLL.Services.Interfaces;

public interface IChatService
{
    Task<Chat> GetOrCreateChatIfNotExistAsync(long chatId);
}