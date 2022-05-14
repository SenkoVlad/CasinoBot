using System.Data.SqlClient;
using Casino.Configuration.Configuration;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Dapper;

namespace Casino.DAL.Repositories.Implementation;

public class PaymentsRepo : IPaymentsRepo
{
    private readonly string _connectionString;

    public PaymentsRepo(IAppConfiguration appConfiguration)
    {
        _connectionString = appConfiguration.DbConnectionString;
    }

    public void AddPayment(Payment payment)
    {
        using var db = new SqlConnection(_connectionString);
        var sqlQuery = "INSERT INTO dbo.Payments (paymentId, chatId, currencyId, totalAmount) VALUES (@paymentId, @chatId, @currencyId, @totalAmount)";
        db.Execute(sqlQuery, payment);
    }
}