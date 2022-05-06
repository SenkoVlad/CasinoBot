using Casino.BLL.Models;
using Casino.DAL.Models;

namespace Casino.BLL.Services.Interfaces;

public interface IChatService
{
    Task<Chat> GetOrCreateChatIfNotExistAsync(long chatId);
    Task<double> ChangeBalanceAsync(GameModel gameModel);
    Task<Chat> GetChatByIdOrException(long chatId);
}