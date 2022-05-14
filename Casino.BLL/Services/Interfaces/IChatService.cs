using Casino.BLL.Models;

namespace Casino.BLL.Services.Interfaces;

public interface IChatService
{
    Task<ChatModel> GetOrCreateChatIfNotExistAsync(long chatId);
    Task<double> ChangeBalanceAsync(GameModel gameModel);
    Task<ChatModel> GetChatByIdOrException(long chatId);
}