using Casino.Configuration.Configuration;
using Casino.TelegramConsumer.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Casino.TelegramConsumer;

class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Bot started!");
        var hosting = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
                        services.AddSingleton<IAppConfiguration, AppConfiguration>()
                                .AddSingleton<IMessageBusSubscriberClient, MessageBusSubscriberClient>())
                    .Build();

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        using IServiceScope serviceScope = hosting!.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;
        var busSubscriberClient = provider.GetRequiredService<IMessageBusSubscriberClient>();
        var configuration = provider.GetRequiredService<IAppConfiguration>();

        var bot = new TelegramBotClient(configuration.BotApiToken);
        await busSubscriberClient.StartListening(bot);

        await hosting.RunAsync(token: cancellationToken);
        Console.ReadLine();
    }
}