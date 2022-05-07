using Casino.BLL.ButtonsGenerators;
using Casino.BLL.Services.Implementation;
using Casino.BLL.Services.Interfaces;
using Casino.Configuration.Configuration;
using Casino.DAL.Repositories.Implementation;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Casino.Telegram;

class Program
{
    public static void Main()
    {
        Console.WriteLine("Bot started!");

        var hosting = Host.CreateDefaultBuilder()
            .ConfigureServices(service => service
                    .AddSingleton<IChatsLanguagesInMemoryRepository, ChatsLanguagesInMemoryRepository>()
                    .AddSingleton<IAppConfiguration, AppConfiguration>()
                    .AddSingleton<IGamesRepo, GamesRepo>()
                    .AddSingleton<IBettingResultsRepo, BettingResultsRepo>()
                    .AddSingleton<GameParameters>()

                    .AddScoped<InlineKeyboardButtonsGenerator>()
                    .AddScoped<IChatService, ChatService>()
                    .AddScoped<IWithdrawService, WithdrawService>()
                    .AddScoped<IWithdrawRequestsRepo, WithdrawRequestsRepo>()
                    .AddScoped<IChatsRepository, ChatsRepository>()
                    .AddScoped<IGameResultsRepo, GameResultsRepo>()
                    .AddLocalization())
            .Build();

        var telegramBot = new TelegramBot(hosting);
        telegramBot.StartReceiving();

        hosting.Run();
        Console.ReadLine();
    }
}