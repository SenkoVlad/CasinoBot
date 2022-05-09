using Casino.BLL.Models;
using Casino.DAL.Models;

namespace Casino.BLL.Services.Interfaces;

public interface IChatService
{
    Task<ChatModel> GetOrCreateChatIfNotExistAsync(long chatId);
    Task<double> ChangeBalanceAsync(GameModel gameModel, BettingResult bettingResult);
    Task<ChatModel> GetChatByIdOrException(long chatId);
}