using Casino.DAL.Models;

namespace Casino.DAL.Repositories.Interfaces;

public interface IPaymentsRepo
{
    void AddPayment(Payment payment);
}