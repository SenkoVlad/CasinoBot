using Casino.BLL.Models;

namespace Casino.BLL.Services.Interfaces;

public interface IPaymentService
{
    Task SavePaymentAsync(PaymentModel paymentModel);
}