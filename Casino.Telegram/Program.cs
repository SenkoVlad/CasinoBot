using System.Globalization;
using Casino.BLL.ButtonsGenerators;
using Casino.BLL.ClickHandlers.Implementation;
using Casino.BLL.Extensions;
using Casino.BLL.Services.Implementation;
using Casino.BLL.Services.Interfaces;
using Casino.Common.AppConstants;
using Casino.Common.Dtos;
using Casino.Common.Enum;
using Casino.DAL.Repositories.Implementation;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using MessageType = Casino.Common.Enum.MessageType;

namespace Casino.Telegram;

class Program
{
    private static IHost? _hosting;
    private static readonly ITelegramBotClient Bot = new TelegramBotClient(AppConstants.BotToken);

    public static async Task Main()
    {
        Console.WriteLine("Bot started!");

        _hosting = Host.CreateDefaultBuilder()
            .ConfigureServices(service => service
                    .AddSingleton<IBalanceRepository, BalanceRepository>()
                    .AddSingleton<IChatsLanguagesInMemoryRepository, ChatsLanguagesInMemoryRepository>()
                    .AddScoped<InlineKeyboardButtonsGenerator>()
                    .AddScoped<IChatService, ChatService>()
                    .AddScoped<IChatRepository, ChatRepository>(_ => new ChatRepository(AppConstants.DbConnectionString))
                    .AddLocalization())
            .Build();

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var receiverOption = new ReceiverOptions();
        Bot.StartReceiving(
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
        Task.Run(async () =>
        {
            try
            {
                using var serviceScope = _hosting!.Services.CreateScope();
                var provider = serviceScope.ServiceProvider;
                var telegramMessage = GetTelegramMessageDto(newMessage);

                SetChatLanguage(provider, telegramMessage.ChatId);

                switch (telegramMessage.MessageType)
                {
                    case MessageType.Start:
                    case MessageType.Callback:
                        await ProcessButtonClickAsync(telegramMessage, Bot, provider);
                        break;
                    case MessageType.UserMessage:
                        await Bot.DeleteMessageAsync(telegramMessage.ChatId,
                            telegramMessage.MessageId, cancellationToken);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                await Bot.AnswerCallbackQueryAsync(newMessage.CallbackQuery!.Id, cancellationToken: cancellationToken);
            }
        }, cancellationToken);
    }

    private static void SetChatLanguage(IServiceProvider provider, long chatId)
    {
        var languagesInMemory = provider.GetRequiredService<IChatsLanguagesInMemoryRepository>();
        var chatLanguage = languagesInMemory.GetChatLanguage(chatId);
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(chatLanguage);
    }

    private static async Task ProcessButtonClickAsync(TelegramMessageDto telegramMessage, ITelegramBotClient telegramBotClient,
        IServiceProvider serviceProvider)
    {
        var inlineButtonPushHandler = new ButtonClickHandler(
            telegramMessage,
            telegramBotClient,
            serviceProvider);
        await inlineButtonPushHandler.PushButtonAsync();
    }

    private static TelegramMessageDto GetTelegramMessageDto(Update newMessage)
    {
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
        }
        else if (IsUpdateTypeCallBackAndCallbackNotNull(newMessage))
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

        return telegramMessage;
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