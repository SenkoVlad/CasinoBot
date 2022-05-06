using System.Data.SqlClient;
using System.Transactions;
using Casino.Configuration.Configuration;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Dapper;

namespace Casino.DAL.Repositories.Implementation;

public class WithdrawRepo : IWithdrawRepo
{
    private readonly string _connectionString;

    public WithdrawRepo(IAppConfiguration appConfiguration)
    {
        _connectionString = appConfiguration.DbConnectionString;
    }
    public async Task<IEnumerable<WithdrawRequest>> GetUnAccountedByChatId(long chatId)
    {
        await using var db = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM dbo.WithdrawRequests AS W WHERE W.chatId = @chatId AND W.isAccounted = 0";
        var withdrawRequests = await db.QueryAsync<WithdrawRequest>(sql, new {chatId});
        return withdrawRequests;
    }

    public async Task InsertAsync(WithdrawRequest withdrawRequest)
    {
        await using var db = new SqlConnection(_connectionString);
        var sqlQuery = @"INSERT INTO dbo.WithdrawRequests (chatId, currencyId, amount, isAccounted, createdDateTimeUtc)
                        VALUES (@chatId, @currencyId, @amount, @isAccounted, @createdDateTimeUtc)";
        await db.ExecuteAsync(sqlQuery, withdrawRequest);
    }
}