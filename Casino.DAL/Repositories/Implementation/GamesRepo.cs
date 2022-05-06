using System.Data.SqlClient;
using Casino.Configuration.Configuration;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Dapper;

namespace Casino.DAL.Repositories.Implementation;

public class GamesRepo : IGamesRepo
{
    private readonly string _connectionString;


    public GamesRepo(IAppConfiguration appConfiguration)
    {
        _connectionString = appConfiguration.DbConnectionString;
    }

    public IEnumerable<Game> GetAllGames()
    {
        using var db = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM dbo.Games";
        var games = db.Query<Game>(sql);
        return games;
    }
}