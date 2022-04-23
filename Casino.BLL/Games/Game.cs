using Casino.DAL.Repositories.Interfaces;

namespace Casino.BLL.Games;

public abstract class Game
{
    protected bool DidWin;
    protected int UserBet;
    
    private readonly long _chatId;
    private readonly IBalanceRepository _balanceRepository;

    protected Game(long chatId, IBalanceRepository balanceRepository, int userBet)
    {
        _chatId = chatId;
        _balanceRepository = balanceRepository;
        UserBet = userBet;
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
            ? _balanceRepository.AddScoreToBalanceAsync(_chatId, UserBet)
            : _balanceRepository.AddScoreToBalanceAsync(_chatId, -UserBet);

        return Task.CompletedTask;
    }

    protected abstract Task InitGameAsync();
    protected abstract Task SentStartMessageAsync();
    protected abstract Task PlayGameRoundAsync();
    protected abstract bool GetRoundResult();
    protected abstract Task SendRoundResultMessageAsync();
}