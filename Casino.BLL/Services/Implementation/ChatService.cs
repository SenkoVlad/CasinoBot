using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;

namespace Casino.BLL.Services.Implementation;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly GameParameters _gameParameters;

    public ChatService(IChatRepository chatRepository,
        GameParameters gameParameters)
    {
        _chatRepository = chatRepository;
        _gameParameters = gameParameters;
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

        return chat;
    }

    public async Task ChangeBalanceAsync(GameModel gameModel)
    {
        var bettingResult = _gameParameters.BettingResults.FirstOrDefault(b =>
            b.IsWin == gameModel.DidWin && 
            b.GameId == gameModel.GameId);

        if (bettingResult == null)
        {
            throw new Exception("Something wrong, try again");
        }

        var score = gameModel.DidWin
            ? bettingResult.Coefficient * gameModel.UserBet
            : -bettingResult.Coefficient * gameModel.UserBet;

        if (gameModel.IsDemoPlay)
        {
            await _chatRepository.ChangeDemoBalanceAsync(gameModel.Chat.Id, score);
        }
        else
        {
            await _chatRepository.ChangeBalanceAsync(gameModel.Chat.Id, score);
        }
    }

    public async Task<Chat> GetChatByIdOrException(long chatId)
    {
        var chat = await _chatRepository.GetChatByIdAsync(chatId);
        if (chat == null)
        {
            throw new EntityNotFoundException($"Chat with id {chatId} is not found");
        }

        return chat;
    }
}

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message)
    {
        Message = message;
    }

    public override string Message { get; }
}