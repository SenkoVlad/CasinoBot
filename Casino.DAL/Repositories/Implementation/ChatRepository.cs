using System.Data.SqlClient;
using Casino.Configuration.Configuration;
using Casino.DAL.DataModels;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Dapper;

namespace Casino.DAL.Repositories.Implementation;

public class ChatRepository : IChatRepository
{
    private readonly string _connectionString;

    public ChatRepository(IAppConfiguration appConfiguration)
    {
        _connectionString = appConfiguration.DbConnectionString;
    }

    public async Task<IEnumerable<ChatDataModel>> GetChatsLanguagesAsync()
    {
        try
        {
            await using var db = new SqlConnection(_connectionString);
            var sql = "SELECT C.Id, C.language FROM dbo.Chats AS C";
            var languages = await db.QueryAsync<ChatDataModel>(sql);
            return languages;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task UpdateChatLanguageAsync(long chatId, string language)
    {
        try
        {
            await using var db = new SqlConnection(_connectionString);
            var sqlQuery = "UPDATE dbo.Chats SET language=@language WHERE id=@id";
            await db.ExecuteAsync(sqlQuery, new {language, id = chatId});
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task AddAsync(Chat chat)
    {
        try
        {
            await using var db = new SqlConnection(_connectionString);
            var sqlQuery = "INSERT INTO Chats (id, language, balance, demoBalance) VALUES (@id, @language, @balance, @demoBalance)";
            await db.ExecuteAsync(sqlQuery, chat);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            throw;
        }
    }

    public async Task<Chat> GetChatByIdAsync(long chatId)
    {
        try
        {
            await using var db = new SqlConnection(_connectionString);
            var chat = await db.QueryFirstOrDefaultAsync<Chat>("SELECT * FROM dbo.Chats WHERE Chats.Id = @id", new { id = chatId });
            return chat;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task ChangeDemoBalanceAsync(long chatId, double score)
    {
        try
        {
            await using var db = new SqlConnection(_connectionString);
            var sqlQuery = "UPDATE dbo.Chats SET demoBalance = demoBalance + @score WHERE Id = @chatId";
            await db.ExecuteAsync(sqlQuery, new { chatId, score });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task ChangeBalanceAsync(long chatId, double score)
    {
        try
        {
            await using var db = new SqlConnection(_connectionString);
            var sqlQuery = "UPDATE dbo.Chats SET balance = balance + @score WHERE Id = @chatId";
            await db.ExecuteAsync(sqlQuery, new { chatId, score });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}