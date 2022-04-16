using Casino.BLL.Models;
using Casino.BLL.ScreenHandlers.Implementation;
using Casino.Common.AppConstants;
using Casino.DAL.Repositories.Implementation;
using Casino.DAL.Repositories.Interfaces;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Casino.TelegramUI;

class Program
{
    private static readonly ITelegramBotClient Bot = new TelegramBotClient(AppConstants.BotToken);
    private static IHost? Hosting;

    public static async Task Main()
    {
        //test1
        //test2
        Console.WriteLine("Bot started!");

        Hosting = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
                services.AddSingleton<IBalanceRepository, BalanceRepository>())
            .Build();

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var receiverOption = new ReceiverOptions();
        Bot.StartReceiving(
            HandleUpdate,
            HandleUpdateError,
            receiverOption,
            cancellationToken);

        await Hosting.RunAsync(token: cancellationToken);

        Console.ReadLine();
    }

    private static Task HandleUpdateError(ITelegramBotClient telegramBotClient, 
        Exception exception, CancellationToken cancellationToken)
    {
        var messageString = JsonConvert.SerializeObject(exception.Message);
        Console.WriteLine(messageString);
        throw new NotImplementedException();
    }

    private static Task HandleUpdate(ITelegramBotClient telegramBotClient, 
        Update newMessage, CancellationToken cancellationToken)
    {
        ProcessMessage(telegramBotClient, newMessage, cancellationToken);
        return Task.CompletedTask;
    }

    private static void ProcessMessage(ITelegramBotClient telegramBotClient, Update newMessage,
        CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
                try
                {
                    var telegramMessage = new TelegramMessage
                    {
                        TelegramBotClient = telegramBotClient,
                        Message = newMessage
                    };
                    var messageJson = JsonConvert.SerializeObject(telegramMessage);

                    using IServiceScope serviceScope = Hosting!.Services.CreateScope();
                    IServiceProvider provider = serviceScope.ServiceProvider;

                    var message = newMessage.Message;

                    if (IsItStartMessageAndNotNull(message))
                    {
                        await InitializeBotWithInlineButtons(message!, provider, cancellationToken);
                        return;
                    }

                    if (IsUpdateTypeCallBackAndCallbackNotNull(newMessage))
                    {
                        var commandModel = GetCommandModel(newMessage);
                        var inlineButtonPushHandler = new InlineScreenHandler(newMessage.CallbackQuery!.Message!,
                            telegramBotClient, cancellationToken, provider);
                        await inlineButtonPushHandler.PushButtonAsync(commandModel);
                    }
                    else if (IsUpdateTypeMessageAndMessageNotNull(newMessage))
                    {
                        await Bot.DeleteMessageAsync(message!.Chat.Id, message!.MessageId, cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
        }
        , cancellationToken);
    }

    private static CommandModel GetCommandModel(Update newMessage)
    {
        var commandJson = newMessage.CallbackQuery!.Data;
        var commandModel = JsonConvert.DeserializeObject<CommandModel>(commandJson);
        return commandModel;
    }

    private static async Task InitializeBotWithInlineButtons(Message message, IServiceProvider provider, CancellationToken cancellationToken)
    {
        var inlineButtonPushHandler = new InlineScreenHandler(message,
            Bot, cancellationToken, provider);
        var commandModel = new CommandModel
        {
            CommandText = AppConstants.StartCommand
        };
        await inlineButtonPushHandler.PushButtonAsync(commandModel);
    }

    private static bool IsUpdateTypeCallBackAndCallbackNotNull(Update newMessage) =>
         newMessage.Type == UpdateType.CallbackQuery && newMessage.CallbackQuery != null;

    private static bool IsUpdateTypeMessageAndMessageNotNull(Update newMessage) =>
        newMessage.Type == UpdateType.Message && newMessage.Message != null;

    private static bool IsItStartMessageAndNotNull(Message? message) =>
        message != null && message.Text?.ToLower() == "/start";
}

public class TelegramMessage
{
    public ITelegramBotClient? TelegramBotClient { get; set; }
    public Update? Message { get; set; }
}




