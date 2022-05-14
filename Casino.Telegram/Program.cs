using System.Reflection;
using AutoMapper;
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
                    .AddSingleton<ICurrenciesRepo, CurrenciesRepo>()
                    .AddSingleton<GameParameters>()

                    .AddScoped<InlineKeyboardButtonsGenerator>()
                    .AddScoped<IPaymentService, PaymentService>()
                    .AddScoped<IChatService, ChatService>()
                    .AddScoped<IWithdrawService, WithdrawService>()
                    .AddScoped<IWithdrawRequestsRepo, WithdrawRequestsRepo>()
                    .AddScoped<IChatsRepository, ChatsRepository>()
                    .AddScoped<IGameResultsRepo, GameResultsRepo>()
                    .AddScoped<IPaymentsRepo, PaymentsRepo>()
                    .AddLocalization()
            
                    .AddAutoMapper(m => m.AddMaps(GetSolutionAssemblies())))
            .Build();



        var telegramBot = new TelegramBot(hosting);
        telegramBot.StartReceiving();

        hosting.Run();
        Console.ReadLine();
    }

    private static Assembly[] GetSolutionAssemblies()
    {
        var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)));
        return assemblies.ToArray();
    }

}