using Casino.DAL.Repositories.Interfaces;
using Telegram.Bot.Types;

namespace Casino.BLL.Games;

public abstract class Game
{
    private readonly IBalanceRepository _balanceRepository;
    private bool _didWin;
    private readonly Message _message;

    protected Game(Message message, IBalanceRepository balanceRepository)
    {
        _message = message;
        _balanceRepository = balanceRepository;
    }

    public virtual async Task PlayRoundAsync()
    {
        await SentStartMessageAsync();
        await PlayGameRoundAsync();

        await Task.Delay(3500);

        SetRoundResult();
        await UpdateBalanceAsync();
        await SendRoundResultMessageAsync();
        await InitGameAsync();
    }
    public virtual Task UpdateBalanceAsync()
    {
        var updateBalanceResult = _didWin
            ? _balanceRepository.AddScoreToBalanceAsync(_message.Chat.Id, 10)
            : _balanceRepository.AddScoreToBalanceAsync(_message.Chat.Id, -10);

        return Task.CompletedTask;
    }

    protected abstract Task InitGameAsync();
    protected abstract Task SentStartMessageAsync();
    protected abstract Task PlayGameRoundAsync();
    protected abstract void SetRoundResult();
    protected abstract Task SendRoundResultMessageAsync();
}