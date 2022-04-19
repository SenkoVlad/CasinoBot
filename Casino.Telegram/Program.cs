﻿using Casino.Common.AppConstants;
using Casino.Configuration.Configuration;
using Casino.DAL.Repositories.Implementation;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Casino.TelegramConsumer;

class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Bot started!");
        var bot = new TelegramBotClient(AppConstants.BotToken);
        var hosting = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
                        services.AddScoped<IBalanceRepository, BalanceRepository>()
                                .AddSingleton<IAppConfiguration, AppConfiguration>()
                                .AddSingleton<IMessageBusSubscriberClient, MessageBusSubscriberClient>())
                    .Build();

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        using IServiceScope serviceScope = hosting!.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;
        var busSubscriberClient = provider.GetRequiredService<IMessageBusSubscriberClient>();
        await busSubscriberClient.StartListening(bot);

        await hosting.RunAsync(token: cancellationToken);
        Console.ReadLine();
    }
}