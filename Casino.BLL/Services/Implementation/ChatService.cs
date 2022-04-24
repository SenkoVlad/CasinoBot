using Casino.BLL.Services.Interfaces;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;

namespace Casino.BLL.Services.Implementation;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Chat> GetOrCreateChatIfNotExistAsync(long chatId)
    {
        var chat = await _chatRepository.GetChatByIdAsync(chatId);
        if (chat == null)
        {
            await _chatRepository.AddAsync(new Chat
            {
                Id = chatId
            });
            chat = await _chatRepository.GetChatByIdAsync(chatId);
        }

        return chat!;
    }
}