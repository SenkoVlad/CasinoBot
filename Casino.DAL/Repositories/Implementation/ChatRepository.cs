﻿using System.Data.SqlClient;
using Casino.Configuration.Configuration;
using Casino.DAL.DataModels;
using Casino.DAL.Exceptions;
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
        await using var db = new SqlConnection(_connectionString);
        var sql = "SELECT C.Id, C.language FROM dbo.Chats AS C";
        var languages = await db.QueryAsync<ChatDataModel>(sql);
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
        var sqlQuery = "INSERT INTO dbo.Chats (id, language, balance, demoBalance) VALUES (@id, @language, @balance, @demoBalance)";
        await db.ExecuteAsync(sqlQuery, chat);
    }

    public async Task<Chat> GetChatByIdAsync(long chatId)
    {
        await using var db = new SqlConnection(_connectionString);
        var chat = await db.QueryFirstOrDefaultAsync<Chat>("SELECT * FROM dbo.Chats WHERE Chats.Id = @id", new { id = chatId });

        if (chat == null)
        {
            throw new EntityNotFoundException($"Chat with id {chatId} is not found");
        }

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