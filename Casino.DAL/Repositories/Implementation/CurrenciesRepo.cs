using System.Data.SqlClient;
using Casino.Configuration.Configuration;
using Casino.DAL.Repositories.Interfaces;
using Dapper;

namespace Casino.DAL.Repositories.Implementation;

public class CurrenciesRepo : ICurrenciesRepo
{
    private readonly string _connectionString;

    public CurrenciesRepo(IAppConfiguration appConfiguration)
    {
        _connectionString = appConfiguration.DbConnectionString;
    }
    public IEnumerable<Models.Currency> GetAllCurrencies()
    {
        using var db = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM dbo.Currencies";
        var currencies = db.Query<Models.Currency>(sql);
        return currencies;
    }
}