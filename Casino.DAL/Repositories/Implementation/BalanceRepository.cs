using Casino.DAL.Repositories.Interfaces;

namespace Casino.DAL.Repositories.Implementation;

public class BalanceRepository : IBalanceRepository
{

    private int _balance;

    public BalanceRepository()
    {
        _balance = 100;
    }

    public int GetBalanceAsync(long chatId) => _balance;

    public bool AddScoreToBalanceAsync(long chatId, int newBalance)
    {
        _balance += newBalance;
        return true;
    }
}