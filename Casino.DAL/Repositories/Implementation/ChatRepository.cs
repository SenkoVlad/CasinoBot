using Casino.DAL.Repositories.Interfaces;

namespace Casino.DAL.Repositories.Implementation;

public class ChatRepository : IChatRepository
{
    public Dictionary<long, string> GetChatsLanguages()
    {
        return new Dictionary<long, string>
        {
            { 194245484, "ru-RU" },
            { 5280713563, "en-US" }
        };
    }

    public Task UpdateChatLanguageAsync(long chatId, string language)
    {
        return  Task.CompletedTask;
    }
}