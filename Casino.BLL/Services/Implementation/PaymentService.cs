using System.Text;
using System.Transactions;
using AutoMapper;
using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;
using Casino.Common.AppConstants;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Casino.BLL.Services.Implementation;

public class PaymentService : IPaymentService
{
    private readonly IPaymentsRepo _paymentsRepo;
    private readonly IMapper _mapper;
    private readonly IChatsRepository _chatsRepository;
    private readonly GameParameters _gameParameters;

    public PaymentService(IPaymentsRepo paymentsRepo,
        IMapper mapper, IChatsRepository chatsRepository,
        GameParameters gameParameters)
    {
        _paymentsRepo = paymentsRepo;
        _mapper = mapper;
        _chatsRepository = chatsRepository;
        _gameParameters = gameParameters;
    }

    public async Task SavePaymentAsync(PaymentModel paymentModel)
    {
        try
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var payment = _mapper.Map<Payment>(paymentModel);
            _paymentsRepo.AddPayment(payment);
            var currency = _gameParameters.Currencies.FirstOrDefault(c => c.Id == payment.CurrencyId);

            if (currency == null)
            {
                throw new Exception($"{nameof(SavePaymentAsync)} currency is not found");
            }

            var amount = currency.Coefficient * payment.TotalAmount;
            await _chatsRepository.ChangeBalanceAsync(payment.ChatId, amount);
            transactionScope.Complete();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<HttpResponseMessage> ConfirmPayment(string preCheckoutQueryId)
    {
        var confirmDepositJson = new ConfirmDepositModel
        {
            pre_checkout_query_id = preCheckoutQueryId,
            ok = true
        };
        var content = new StringContent(JsonConvert.SerializeObject(confirmDepositJson), Encoding.UTF8, "application/json");
        var httpClient = new HttpClient();
        var depositResult = await httpClient.PostAsync(new Uri(AppConstants.TelegramApiConfirmDeposit), content);
        return depositResult;
    }
}