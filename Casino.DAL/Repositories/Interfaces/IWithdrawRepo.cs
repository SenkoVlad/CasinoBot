using Casino.DAL.Models;

namespace Casino.DAL.Repositories.Interfaces;

public interface IWithdrawRepo
{
    Task InsertAsync(WithdrawRequest request);
}