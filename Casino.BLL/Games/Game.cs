using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;

namespace Casino.BLL.Games;

public abstract class Game
{
    private readonly GameModel _gameModel;
    private readonly IChatService _chatService;

    protected Game(GameModel gameModel, IChatService chatService)
    {
        _gameModel = gameModel;
        _chatService = chatService;
    }

    public virtual async Task PlayRoundAsync()
    {
        await SentStartMessageAsync();
        await PlayGameRoundAsync();

        await Task.Delay(3500);

        _gameModel.DidWin = GetRoundResult();
        await UpdateBalanceAsync();
        await SendRoundResultMessageAsync();

        if (_gameModel.IsDemoPlay)
        {
            await InitDemoGameAsync();
        }
        else
        {
            await InitRealGameAsync();
        }
    }


    public virtual async Task UpdateBalanceAsync()
    {
        await _chatService.ChangeBalanceAsync(_gameModel);
    }

    protected abstract Task InitDemoGameAsync();
    protected abstract Task InitRealGameAsync();
    protected abstract Task SentStartMessageAsync();
    protected abstract Task PlayGameRoundAsync();
    protected abstract bool GetRoundResult();
    protected abstract Task SendRoundResultMessageAsync();
}