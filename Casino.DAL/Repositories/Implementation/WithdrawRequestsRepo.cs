using System.Data.SqlClient;
using Casino.Configuration.Configuration;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Dapper;

namespace Casino.DAL.Repositories.Implementation;

public class WithdrawRequestsRepo : IWithdrawRequestsRepo
{
    private readonly string _connectionString;

    public WithdrawRequestsRepo(IAppConfiguration appConfiguration)
    {
        _connectionString = appConfiguration.DbConnectionString;
    }

    public async Task InsertAsync(WithdrawRequest withdrawRequest)
    {
        await using var db = new SqlConnection(_connectionString);
        var sqlQuery = @"INSERT INTO dbo.WithdrawRequests (chatId, currencyId, amount, isAccounted, createdDateTimeUtc)
                        VALUES (@chatId, @currencyId, @amount, @isAccounted, @createdDateTimeUtc)";
        await db.ExecuteAsync(sqlQuery, withdrawRequest);
    }
}