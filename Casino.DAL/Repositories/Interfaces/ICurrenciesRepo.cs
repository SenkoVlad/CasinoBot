namespace Casino.DAL.Repositories.Interfaces;

public interface ICurrenciesRepo
{
    IEnumerable<Models.Currency> GetAllCurrencies();
}