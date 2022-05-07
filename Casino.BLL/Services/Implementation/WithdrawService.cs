using System.Transactions;
using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;
using Casino.Common.AppConstants;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Localization;

namespace Casino.BLL.Services.Implementation;

public class WithdrawService : IWithdrawService
{
    private readonly IWithdrawRequestsRepo _withdrawRequestsRepo;
    private readonly IChatsRepository _chatsRepository;
    private readonly IStringLocalizer<Resources> _localizer;


    public WithdrawService(IWithdrawRequestsRepo withdrawRequestsRepo,
        IChatsRepository chatsRepository, 
        IStringLocalizer<Resources> localizer)
    {
        _withdrawRequestsRepo = withdrawRequestsRepo;
        _chatsRepository = chatsRepository;
        _localizer = localizer;
    }

    public async Task<WithdrawResult> WithdrawAsync(WithdrawModel withdrawModel, long chatId)
    {
        var withdrawResult = new WithdrawResult();

        try
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var chat = await _chatsRepository.GetChatByIdAsync(chatId);

            if (IsBalanceEnoughToWithdraw(chat.Balance, withdrawModel.Amount))
            {
                await _withdrawRequestsRepo.InsertAsync(new WithdrawRequest
                {
                    ChatId = chatId,
                    Amount = withdrawModel.Amount,
                    CreatedDateTimeUtc = DateTime.UtcNow,
                    IsAccounted = false,
                    CurrencyId = (int) withdrawModel.Method
                });
                await _chatsRepository.ChangeBalanceAsync(chatId, -withdrawModel.Amount);

                withdrawResult.IsSuccess = true;
                withdrawResult.Message = _localizer[Resources.WithdrawSuccess];
            }
            else
            {
                withdrawResult.IsSuccess = false;
                withdrawResult.Message = _localizer[Resources.BalanceIsNotEnoughToWithdraw];
            }

            transactionScope.Complete();
            return withdrawResult;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            withdrawResult.IsSuccess = false;
            withdrawResult.Message = _localizer[Resources.SomethingWrongTryAgain];
            return withdrawResult;
        }
    }

    private bool IsBalanceEnoughToWithdraw(double chatBalance, int withdrawAmount) => chatBalance >= withdrawAmount;
}