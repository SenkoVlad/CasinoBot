using Casino.DAL.Repositories.Interfaces;

namespace Casino.DAL.Repositories.Implementation;

public class BalanceRepository : IBalanceRepository
{
    public int GetBalanceAsync(long chatId)
    {
        return 100;
    }
}