using Casino.Common.AppConstants;
using Casino.DAL.Repositories.Interfaces;

namespace Casino.DAL.Repositories.Implementation;

public class ChatsLanguagesInMemoryRepository : IChatsLanguagesInMemoryRepository
{
    private readonly Dictionary<long, string> _usersLanguages;

    public ChatsLanguagesInMemoryRepository(IChatRepository chatRepository)
    {
        _usersLanguages = chatRepository.GetChatsLanguages();
    }

    public void AddOrUpdateChatLanguage(long chatId, string language)
    {
        if (_usersLanguages.ContainsKey(chatId))
        {
            _usersLanguages[chatId] = language;
        }
        else
        {
            _usersLanguages.Add(chatId, language);
        }
    }

    public string GetChatLanguage(long chatId)
    {
        _usersLanguages.TryGetValue(chatId, out var language);
        return language ?? AppConstants.DefaultLanguage;
    }
}