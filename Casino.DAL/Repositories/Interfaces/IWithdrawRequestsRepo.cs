using Casino.DAL.Models;

namespace Casino.DAL.Repositories.Interfaces;

public interface IWithdrawRequestsRepo
{
    Task InsertAsync(WithdrawRequest request);
}