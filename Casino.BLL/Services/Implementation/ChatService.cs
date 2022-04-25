using Casino.BLL.Models;
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

    public async Task ChangeBalanceByIdAsync(GameModel gameModel)
    {
        var score = gameModel.DidWin
            ? gameModel.UserBet
            : -gameModel.UserBet;

        if (gameModel.IsDemoPlay)
        {
            await _chatRepository.ChangeDemoBalanceAsync(gameModel.ChatId, score);
        }
        else
        {
            await _chatRepository.ChangeBalanceAsync(gameModel.ChatId, score);
        }
    }
}