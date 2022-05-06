using Casino.Common.AppConstants;
using Casino.DAL.DataModels;
using Casino.DAL.Repositories.Interfaces;

namespace Casino.DAL.Repositories.Implementation;

public class ChatsLanguagesInMemoryRepository : IChatsLanguagesInMemoryRepository
{
    private readonly List<ChatDataModel> _usersLanguages;

    public ChatsLanguagesInMemoryRepository(IChatRepository chatRepository)
    {
        _usersLanguages = chatRepository.GetChatsLanguagesAsync()
            .GetAwaiter()
            .GetResult()
            .ToList();
    }

    public void AddOrUpdateChatLanguage(long chatId, string language)
    {
        var chat = _usersLanguages.FirstOrDefault(c => c.Id == chatId);

        if (chat == null)
        {
            _usersLanguages.Add(new ChatDataModel
            {
                Id = chatId,
                Language = language
            });
        }
        else
        {
            chat.Language = language;
        }
    }

    public string GetChatLanguage(long chatId)
    {
        var chat = _usersLanguages.FirstOrDefault(c => c.Id == chatId);

        if (chat == null)
        {
            return AppConstants.DefaultLanguage;
        }

        return chat.Language;
    }
}