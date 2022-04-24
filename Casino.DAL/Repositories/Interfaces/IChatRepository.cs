using Casino.DAL.Models;

namespace Casino.DAL.Repositories.Interfaces;

public interface IChatRepository
{
    public Dictionary<long, string> GetChatsLanguages();
    public Task UpdateChatLanguageAsync(long chatId, string language);
    public Task AddAsync(Chat chat);
    public Task<Chat?> GetChatByIdAsync(long chatId);
}