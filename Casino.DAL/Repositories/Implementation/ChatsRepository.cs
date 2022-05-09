using System.Data.SqlClient;
using Casino.Configuration.Configuration;
using Casino.DAL.DataModels;
using Casino.DAL.Exceptions;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Dapper;

namespace Casino.DAL.Repositories.Implementation;

public class ChatsRepository : IChatsRepository
{
    private readonly string _connectionString;

    public ChatsRepository(IAppConfiguration appConfiguration)
    {
        _connectionString = appConfiguration.DbConnectionString;
    }

    public async Task<IEnumerable<ChatLanguageDataModel>> GetChatsLanguagesAsync()
    {
        await using var db = new SqlConnection(_connectionString);
        var sql = "SELECT C.Id, C.language FROM dbo.Chats AS C";
        var languages = await db.QueryAsync<ChatLanguageDataModel>(sql);
        return languages;
    }

    public async Task UpdateChatLanguageAsync(long chatId, string language)
    {
        await using var db = new SqlConnection(_connectionString);
        var sqlQuery = "UPDATE dbo.Chats SET language=@language WHERE id=@id";
        await db.ExecuteAsync(sqlQuery, new {language, id = chatId});
    }

    public async Task AddAsync(Chat chat)
    {
        await using var db = new SqlConnection(_connectionString);
        var sqlQuery = "INSERT INTO dbo.Chats (id, language, balance, demoBalance, createDateTimeUtc) VALUES (@id, @language, @balance, @demoBalance, @createDateTimeUtc)";
        await db.ExecuteAsync(sqlQuery, chat);
    }

    public async Task<ChatDataModel> GetChatByIdAsync(long chatId)
    {
        await using var db = new SqlConnection(_connectionString);
        var chat = await db.QueryFirstOrDefaultAsync<ChatDataModel>(@"SELECT C.Id, C.balance, C.demoBalance
                                                                          FROM dbo.Chats AS C WHERE C.Id = @id", new { id = chatId });
        return chat;
    }

    public async Task ChangeDemoBalanceAsync(long chatId, double amount)
    {
        await using var db = new SqlConnection(_connectionString);
        var sqlQuery = "UPDATE dbo.Chats SET demoBalance = demoBalance + @amount WHERE Id = @chatId";
        await db.ExecuteAsync(sqlQuery, new { chatId, amount });
    }

    public async Task ChangeBalanceAsync(long chatId, double amount)
    {
        await using var db = new SqlConnection(_connectionString);
        var sqlQuery = "UPDATE dbo.Chats SET balance = balance + @amount WHERE Id = @chatId";
        await db.ExecuteAsync(sqlQuery, new { chatId, amount });
    }
}