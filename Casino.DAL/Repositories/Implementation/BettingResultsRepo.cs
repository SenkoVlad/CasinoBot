using System.Data.SqlClient;
using Casino.Configuration.Configuration;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Dapper;

namespace Casino.DAL.Repositories.Implementation;

public class BettingResultsRepo : IBettingResultsRepo
{
    private readonly string _connectionString;

    public BettingResultsRepo(IAppConfiguration appConfiguration)
    {
        _connectionString = appConfiguration.DbConnectionString;
    }

    public IEnumerable<BettingResult> GetAllBettingResults()
    {
        try
        {
            using var db = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM dbo.BettingResults";
            var bettingResults = db.Query<BettingResult>(sql);
            return bettingResults;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}