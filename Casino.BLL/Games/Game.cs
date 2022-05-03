using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Casino.BLL.Games;

public abstract class Game
{
    private readonly GameModel _gameModel;
    private readonly IChatService _chatService;

    protected Game(GameModel gameModel, IServiceProvider serviceProvider)
    {
        _gameModel = gameModel;
        _chatService = serviceProvider.GetRequiredService<IChatService>();
    }

    public virtual async Task PlayRoundAsync()
    {
        if (!IsMoneyEnoughToPlay())
        {
            await SendDoNotHaveEnoughMoneyToPlayMessageAsync();
            await InitGameAsync();
            return;
        }

        await SentStartMessageAsync();
        await PlayGameRoundAsync();

        await Task.Delay(3500);

        _gameModel.DidWin = GetRoundResult();
        await UpdateBalanceAsync();
        await SendRoundResultMessageAsync();

        await InitGameAsync();
    }

    private bool IsMoneyEnoughToPlay()
    {
        var isBalanceEnoughToPlay = _gameModel.IsDemoPlay
            ? _gameModel.Chat.DemoBalance >= _gameModel.UserBet
            : _gameModel.Chat.Balance >= _gameModel.UserBet;

        return isBalanceEnoughToPlay;
    }


    public virtual async Task UpdateBalanceAsync()
    {
        await _chatService.ChangeBalanceAsync(_gameModel);
    }

    protected abstract Task SendDoNotHaveEnoughMoneyToPlayMessageAsync();
    protected abstract Task InitGameAsync();
    protected abstract Task SentStartMessageAsync();
    protected abstract Task PlayGameRoundAsync();
    protected abstract bool GetRoundResult();
    protected abstract Task SendRoundResultMessageAsync();
}