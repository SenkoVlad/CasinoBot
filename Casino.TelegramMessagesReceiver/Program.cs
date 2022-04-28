using Casino.Common.AppConstants;
using Casino.Common.Dtos;
using Casino.Common.Enum;
using Casino.Configuration.Configuration;
using Casino.TelegramSender.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MessageType = Casino.Common.Enum.MessageType;

namespace Casino.TelegramSender;

class Program
{
    private static IHost? _hosting;
    public static async Task Main()
    {
        Console.WriteLine("Bot started!");

        ITelegramBotClient bot = new TelegramBotClient(AppConstants.BotToken);
        _hosting = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
                        services.AddSingleton<IMessageBusClient, MessageBusClient>()
                                .AddSingleton<IAppConfiguration, AppConfiguration>())
            .Build();

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var receiverOption = new ReceiverOptions();
        bot.StartReceiving(
            HandleUpdate,
            HandleUpdateError,
            receiverOption,
            cancellationToken);

        await _hosting.RunAsync(token: cancellationToken);
        Console.ReadLine();
    }

    private static Task HandleUpdateError(ITelegramBotClient telegramBotClient,
        Exception exception, CancellationToken cancellationToken)
    {
        var messageString = JsonConvert.SerializeObject(exception.Message);
        Console.WriteLine(messageString);
        return Task.CompletedTask;
    }

    private static Task HandleUpdate(ITelegramBotClient telegramBotClient,
        Update newMessage, CancellationToken cancellationToken)
    {
        ProcessMessage(newMessage, cancellationToken);
        return Task.CompletedTask;
    }

    private static void ProcessMessage(Update newMessage,
        CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            try
            {
                using var serviceScope = _hosting!.Services.CreateScope();
                var provider = serviceScope.ServiceProvider;
                var messageBusClient = provider.GetRequiredService<IMessageBusClient>();

                var message = newMessage.Message;
                var telegramMessage = new TelegramMessageDto();

                if (IsItStartMessageAndNotNull(message))
                {
                    telegramMessage = new TelegramMessageDto
                    {
                        Text = message!.Text,
                        ChatId = message.Chat.Id,
                        MessageId = message.MessageId,
                        MessageType = MessageType.Start,
                        CommandDto = new CommandDto
                        {
                            Command = Command.Start
                        }
                    };
                    messageBusClient.PublishMessage(telegramMessage);
                    return;
                }

                if (IsUpdateTypeCallBackAndCallbackNotNull(newMessage))
                {
                    var callbackMessage = newMessage.CallbackQuery!.Message!;
                    var commandDto = GetCommandDto(newMessage);

                    telegramMessage = new TelegramMessageDto
                    {
                        Text = callbackMessage.Text,
                        ChatId = callbackMessage.Chat.Id,
                        MessageId = callbackMessage.MessageId,
                        CommandDto = commandDto,
                        MessageType = MessageType.Callback
                    };
                }
                else if (IsUpdateTypeMessageAndMessageNotNull(newMessage))
                {
                    telegramMessage = new TelegramMessageDto
                    {
                        ChatId = message!.Chat.Id,
                        MessageId = message.MessageId,
                        MessageType = MessageType.UserMessage
                    };
                }
                messageBusClient.PublishMessage(telegramMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }, cancellationToken);
    }

    private static CommandDto GetCommandDto(Update newMessage)
    {
        var commandJson = newMessage.CallbackQuery!.Data;
        var commandDto = JsonConvert.DeserializeObject<CommandDto>(commandJson);
        return commandDto;
    }

    private static bool IsUpdateTypeCallBackAndCallbackNotNull(Update newMessage) =>
         newMessage.Type == UpdateType.CallbackQuery && newMessage.CallbackQuery != null;

    private static bool IsUpdateTypeMessageAndMessageNotNull(Update newMessage) =>
        newMessage.Type == UpdateType.Message && newMessage.Message != null;

    private static bool IsItStartMessageAndNotNull(Message? message) =>
        message != null && message.Text?.ToLower() == "/start";
}