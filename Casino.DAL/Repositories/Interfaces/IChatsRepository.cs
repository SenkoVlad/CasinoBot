using Casino.DAL.DataModels;
using Casino.DAL.Models;

namespace Casino.DAL.Repositories.Interfaces;

public interface IChatsRepository
{
    public Task<IEnumerable<ChatLanguageDataModel>> GetChatsLanguagesAsync();
    public Task UpdateChatLanguageAsync(long chatId, string language);
    public Task AddAsync(Chat chat);
    public Task<ChatDataModel> GetChatByIdAsync(long chatId);
    Task ChangeDemoBalanceAsync(long chatId, double score);
    Task ChangeBalanceAsync(long chatId, double amount);
}