using System.Text;
using Casino.BLL.Models;
using Newtonsoft.Json;

namespace Casino.BLL.Services.Interfaces;

public interface IPaymentService
{
    Task SavePaymentAsync(PaymentModel paymentModel);
    Task<HttpResponseMessage> ConfirmPayment(string preCheckoutQueryId);
}