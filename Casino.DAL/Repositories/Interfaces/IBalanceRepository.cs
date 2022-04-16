namespace Casino.DAL.Repositories.Interfaces;

public interface IBalanceRepository
{
    int GetBalanceAsync(long chatId);
    bool AddScoreToBalanceAsync(long chatId, int newBalance);
}