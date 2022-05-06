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
    private readonly IWithdrawRepo _withdrawRepo;
    private readonly IChatRepository _chatRepository;
    private readonly IStringLocalizer<Resources> _localizer;


    public WithdrawService(IWithdrawRepo withdrawRepo,
        IChatRepository chatRepository, 
        IStringLocalizer<Resources> localizer)
    {
        _withdrawRepo = withdrawRepo;
        _chatRepository = chatRepository;
        _localizer = localizer;
    }

    public async Task<WithdrawResult> WithdrawAsync(WithdrawModel withdrawModel, long chatId)
    {
        var withdrawResult = new WithdrawResult();

        try
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var chat = await _chatRepository.GetChatByIdAsync(chatId);

            if (IsBalanceEnoughToWithdraw(chat.Balance, withdrawModel.Amount))
            {
                await _withdrawRepo.InsertAsync(new WithdrawRequest
                {
                    ChatId = chatId,
                    Amount = withdrawModel.Amount,
                    CreatedDateTimeUtc = DateTime.UtcNow,
                    IsAccounted = false,
                    CurrencyId = (int) withdrawModel.Method
                });
                await _chatRepository.ChangeBalanceAsync(chatId, -withdrawModel.Amount);

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

public class WithdrawResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}