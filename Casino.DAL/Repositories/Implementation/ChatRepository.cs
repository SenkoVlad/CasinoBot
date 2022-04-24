using System.Data;
using System.Data.SqlClient;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Dapper;

namespace Casino.DAL.Repositories.Implementation;

public class ChatRepository : IChatRepository
{
    private readonly string _connectionString;

    public ChatRepository(string connectionString)
    {
        this._connectionString = connectionString;
    }

    public Dictionary<long, string> GetChatsLanguages()
    {
        return new Dictionary<long, string>
        {
            { 194245484, "ru-RU" },
            { 5280713563, "en-US" }
        };
    }

    public Task UpdateChatLanguageAsync(long chatId, string language)
    {
        return  Task.CompletedTask;
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
            Console.WriteLine(exception);
        }
    }

    public async Task<Chat?> GetChatByIdAsync(long chatId)
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
            return null;
        }
    }
}