namespace Casino.DAL.Repositories.Interfaces;

public interface IChatsLanguagesInMemoryRepository
{
    public void AddOrUpdateChatLanguage(long chatId, string language);
    public string GetChatLanguage(long chatId);

}