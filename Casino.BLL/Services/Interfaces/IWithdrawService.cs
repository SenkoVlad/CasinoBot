using Casino.BLL.Models;
using Casino.BLL.Services.Implementation;

namespace Casino.BLL.Services.Interfaces;

public interface IWithdrawService
{
    Task<WithdrawResult> WithdrawAsync(WithdrawModel withdrawModel, long chatId);
}