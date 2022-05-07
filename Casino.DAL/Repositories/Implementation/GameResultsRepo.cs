using System.Data.SqlClient;
using Casino.Configuration.Configuration;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Dapper;

namespace Casino.DAL.Repositories.Implementation;

public class GameResultsRepo : IGameResultsRepo
{
    private readonly string _connectionString;

    public GameResultsRepo(IAppConfiguration appConfiguration)
    {
        _connectionString = appConfiguration.DbConnectionString;
    }

    public async Task AddAsync(GameResult gameResult)
    {
        await using var db = new SqlConnection(_connectionString);
        var sqlQuery = @"INSERT INTO dbo.GameResults (chatId, bet, bettingResultId)
                         VALUES (@chatId, @bet, @bettingResultId)";
        await db.ExecuteAsync(sqlQuery, gameResult);
    }
}