using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;
using Casino.DAL.Exceptions;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;

namespace Casino.BLL.Services.Implementation;

public class ChatService : IChatService
{
    private readonly IChatsRepository _chatsRepository;

    public ChatService(IChatsRepository chatsRepository)
    {
        _chatsRepository = chatsRepository;
    }

    public async Task<ChatModel> GetOrCreateChatIfNotExistAsync(long chatId)
    {
        var chat = await _chatsRepository.GetChatByIdAsync(chatId);
        if (chat == null)
        {
            await _chatsRepository.AddAsync(new Chat
            {
                Id = chatId,
            });
            chat = await _chatsRepository.GetChatByIdAsync(chatId);
        }

        return new ChatModel
        {
            Id = chat.Id,
            Balance = chat.Balance,
            DemoBalance = chat.DemoBalance
        };
    }

    public async Task<double> ChangeBalanceAsync(GameModel gameModel)
    {
        var score = gameModel.BettingResult.IsWon
            ? gameModel.BettingResult!.Coefficient * gameModel.UserBet
            : -gameModel.BettingResult.Coefficient * gameModel.UserBet;

        if (gameModel.IsDemoPlay)
        {
            await _chatsRepository.ChangeDemoBalanceAsync(gameModel.Chat.Id, score);
        }
        else
        {
            await _chatsRepository.ChangeBalanceAsync(gameModel.Chat.Id, score);
        }

        return score;
    }

    public async Task<ChatModel> GetChatByIdOrException(long chatId)
    {
        try
        {
            var chat = await _chatsRepository.GetChatByIdAsync(chatId);
            
            if (chat == null)
            {
                throw new Common.Exceptions.EntityNotFoundException($"Not found chat with id {chatId}");
            }

            return new ChatModel
            {
                Id = chat.Id,
                Balance = chat.Balance,
                DemoBalance = chat.DemoBalance
            };
        }
        catch (EntityNotFoundException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}