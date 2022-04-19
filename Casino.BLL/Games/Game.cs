using Casino.DAL.Repositories.Interfaces;

namespace Casino.BLL.Games;

public abstract class Game
{
    protected bool DidWin;

    private readonly IBalanceRepository _balanceRepository;
    private readonly long _chatId;

    protected Game(long chatId, IBalanceRepository balanceRepository)
    {
        _chatId = chatId;
        _balanceRepository = balanceRepository;
    }

    public virtual async Task PlayRoundAsync()
    {
        await SentStartMessageAsync();
        await PlayGameRoundAsync();

        await Task.Delay(3500);

        DidWin = GetRoundResult();
        await UpdateBalanceAsync();
        await SendRoundResultMessageAsync();
        await InitGameAsync();
    }
    public virtual Task UpdateBalanceAsync()
    {
        var updateBalanceResult = DidWin
            ? _balanceRepository.AddScoreToBalanceAsync(_chatId, 10)
            : _balanceRepository.AddScoreToBalanceAsync(_chatId, -10);

        return Task.CompletedTask;
    }

    protected abstract Task InitGameAsync();
    protected abstract Task SentStartMessageAsync();
    protected abstract Task PlayGameRoundAsync();
    protected abstract bool GetRoundResult();
    protected abstract Task SendRoundResultMessageAsync();
}