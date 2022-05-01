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
        try
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
        catch (Exception e)
        {
            Console.WriteLine($"Method {nameof(AddOrUpdateChatLanguage)}. {e.Message}");
            throw;
        }
    }

    public string GetChatLanguage(long chatId)
    {
        try
        {
            var chat = _usersLanguages.FirstOrDefault(c => c.Id == chatId);

            if (chat == null)
            {
                return AppConstants.DefaultLanguage;
            }

            return chat.Language;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}