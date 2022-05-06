using System.Transactions;
using Casino.DAL.Models;

namespace Casino.DAL.Repositories.Interfaces;

public interface IWithdrawRepo
{
    Task<IEnumerable<WithdrawRequest>> GetUnAccountedByChatId(long chatId);
    Task InsertAsync(WithdrawRequest request);
}