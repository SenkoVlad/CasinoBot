using System.Globalization;
using Casino.BLL.ClickHandlers.Implementation;
using Casino.BLL.Services.Interfaces;
using Casino.Common.Dtos;
using Casino.Common.Enum;
using Casino.Configuration.Configuration;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MessageType = Casino.Common.Enum.MessageType;

namespace Casino.Telegram;

class TelegramBot
{
    private readonly IHost _hosting;
    private readonly ITelegramBotClient _bot;
    public TelegramBot(IHost hosting)
    {
        _hosting = hosting;
        var configuration = hosting.Services.GetRequiredService<IAppConfiguration>();
        _bot = new TelegramBotClient(configuration.BotApiToken);
    }

    public void StartReceiving()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var receiverOption = new ReceiverOptions();

        _bot.StartReceiving(
            HandleUpdate,
            HandleUpdateError,
            receiverOption,
            cancellationToken);
    }

    private Task HandleUpdateError(ITelegramBotClient telegramBotClient,
        Exception exception, CancellationToken cancellationToken)
    {
        var messageString = JsonConvert.SerializeObject(exception.Message);
        Console.WriteLine(messageString);
        return Task.CompletedTask;
    }

    private Task HandleUpdate(ITelegramBotClient telegramBotClient,
        Update newMessage, CancellationToken cancellationToken)
    {
        ProcessMessage(newMessage, cancellationToken);
        return Task.CompletedTask;
    }

    private void ProcessMessage(Update newMessage,
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
                        await ProcessButtonClickAsync(telegramMessage, _bot, provider);
                        break;
                    case MessageType.UserMessage:
                        await _bot.DeleteMessageAsync(telegramMessage.ChatId,
                            telegramMessage.MessageId, cancellationToken);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if(newMessage.CallbackQuery != null)
                    await _bot.AnswerCallbackQueryAsync(newMessage.CallbackQuery!.Id, cancellationToken: cancellationToken);
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