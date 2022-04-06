using Casino.BLL.ButtonPushHandlers.Implementation;
using Casino.BLL.ButtonPushHandlers.Interfaces;
using Casino.BLL.ButtonsHelper;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Casino.TelegramUI;

class Program
{
    private static readonly ITelegramBotClient Bot = new TelegramBotClient("610837243:AAE6tS9UYngU_qzd6-peNQYDslFy-KfygTs");
    private static readonly Dictionary<long, IButtonPushHandler> InlineButtonPushHandlers = new();

    public static void Main()
    {
        Console.WriteLine("Bot started!");
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var receiverOption = new ReceiverOptions();
        Bot.StartReceiving(
            HandleUpdateAsync,
            HandleUpdateError,
            receiverOption,
            cancellationToken);
        
        Console.ReadLine();
    }

    private static Task HandleUpdateError(ITelegramBotClient telegramBotClient, 
        Exception exception, CancellationToken cancellationToken)
    {
        var messageString = JsonConvert.SerializeObject(exception.Message);
        Console.WriteLine(messageString);
        throw new NotImplementedException();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient telegramBotClient, 
        Update newMessage, CancellationToken cancellationToken)
    {
        try
        {
            var message = newMessage.Message;
            
            if (IsItStartMessage(message!))
            {
                await InitialBot(message!);
                return;
            }

            if (IsUpdateTypeMessageAndMessageNotNull(newMessage))
            {
                var chatId = newMessage.Message!.Chat.Id;
                var buttonPushHandler = GetCurrentButtonPushHandler(telegramBotClient, cancellationToken, chatId, message);

                var commandText = message!.Text;
                await buttonPushHandler.PushAsync(commandText);
            }
            else if (IsUpdateTypeCallBackAndCallbackNotNull(newMessage))
            {
                var inlineButtonPushHandler = new InlineButtonPushHandler(telegramBotClient, cancellationToken);
                var commandText = newMessage.CallbackQuery!.Data;
                await inlineButtonPushHandler.PushAsync(commandText);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static KeyboardButtonPushHandler GetCurrentButtonPushHandler(ITelegramBotClient telegramBotClient,
        CancellationToken cancellationToken, long chatId, Message? message)
    {
        KeyboardButtonPushHandler buttonPushHandler;
        if (IsItNewClientChat(chatId))
        {
            buttonPushHandler = new KeyboardButtonPushHandler(message!, telegramBotClient, cancellationToken);
            InlineButtonPushHandlers.Add(chatId, buttonPushHandler);
        }
        else
        {
            buttonPushHandler = (KeyboardButtonPushHandler) InlineButtonPushHandlers[chatId];
        }

        return buttonPushHandler;
    }

    private static bool IsItNewClientChat(long? chatId)
    {
        return chatId  != null && !InlineButtonPushHandlers.ContainsKey((long)chatId);
    }

    private static bool IsUpdateTypeCallBackAndCallbackNotNull(Update newMessage) =>
         newMessage.Type == UpdateType.CallbackQuery && newMessage.CallbackQuery != null;

    private static async Task InitialBot(Message message)
    {
        var startButtons = ButtonsGeneratorHelper.GetStartButtons();

        await Bot.SendTextMessageAsync(message.Chat.Id, "Start...",
            replyToMessageId: message.MessageId, replyMarkup: startButtons);
    }

    private static bool IsUpdateTypeMessageAndMessageNotNull(Update newMessage) =>
        newMessage.Type == UpdateType.Message && newMessage.Message != null;

    private static bool IsItStartMessage(Message message) =>
        message.Text?.ToLower() == "/start";
}

