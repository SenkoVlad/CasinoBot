using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;
using Casino.DAL;
using Casino.DAL.Exceptions;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;

namespace Casino.BLL.Services.Implementation;

public class ChatService : IChatService
{
    private readonly IChatsRepository _chatsRepository;
    private readonly GameParameters _gameParameters;

    public ChatService(IChatsRepository chatsRepository,
        GameParameters gameParameters)
    {
        _chatsRepository = chatsRepository;
        _gameParameters = gameParameters;
    }

    public async Task<Chat> GetOrCreateChatIfNotExistAsync(long chatId)
    {
        var chat = await _chatsRepository.GetChatByIdAsync(chatId);
        if (chat == null)
        {
            await _chatsRepository.AddAsync(new Chat
            {
                Id = chatId
            });
            chat = await _chatsRepository.GetChatByIdAsync(chatId);
        }

        return chat;
    }

    public async Task<double> ChangeBalanceAsync(GameModel gameModel, BettingResult bettingResult)
    {
        var score = gameModel.DidWin
            ? bettingResult.Coefficient * gameModel.UserBet
            : -bettingResult.Coefficient * gameModel.UserBet;

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

    public async Task<Chat> GetChatByIdOrException(long chatId)
    {
        try
        {
            var chat = await _chatsRepository.GetChatByIdAsync(chatId);
            
            if (chat == null)
            {
                throw new Common.Exceptions.EntityNotFoundException($"Not found chat with id {chatId}");
            }

            return chat;
        }
        catch (EntityNotFoundException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}