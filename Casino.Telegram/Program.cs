using Casino.BLL.ButtonsGenerators;
using Casino.BLL.ScreenHandlers.Implementation;
using Casino.BLL.ScreenHandlers.Interfaces;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Casino.TelegramUI;

class Program
{
    private static readonly ITelegramBotClient Bot = new TelegramBotClient("610837243:AAE6tS9UYngU_qzd6-peNQYDslFy-KfygTs");
    private static readonly Dictionary<long, IScreenHandler> ChatScreenHandlers = new();

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
                await InitializeBotWithKeyboardButtons(message!);
                return;
            }

            if (IsUpdateTypeMessageAndMessageNotNull(newMessage))
            {
                var chatId = newMessage.Message!.Chat.Id;
                var buttonPushHandler = GetCurrentScreenHandler(telegramBotClient, cancellationToken, chatId, message);

                var commandText = message!.Text;
                await buttonPushHandler.PushButtonAsync(commandText);
            }
            else if (IsUpdateTypeCallBackAndCallbackNotNull(newMessage))
            {
                var inlineButtonPushHandler = new InlineScreenHandler(telegramBotClient, cancellationToken);
                var commandText = newMessage.CallbackQuery!.Data;
                await inlineButtonPushHandler.PushButtonAsync(commandText);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static KeyboardScreenHandler GetCurrentScreenHandler(ITelegramBotClient telegramBotClient,
        CancellationToken cancellationToken, long chatId, Message? message)
    {
        KeyboardScreenHandler screenHandler;
        if (IsItNewClientChat(chatId))
        {
            screenHandler = new KeyboardScreenHandler(message!, telegramBotClient, cancellationToken);
            ChatScreenHandlers.Add(chatId, screenHandler);
        }
        else
        {
            screenHandler = (KeyboardScreenHandler) ChatScreenHandlers[chatId];
        }

        return screenHandler;
    }

    private static bool IsItNewClientChat(long? chatId)
    {
        return chatId  != null && !ChatScreenHandlers.ContainsKey((long)chatId);
    }

    private static bool IsUpdateTypeCallBackAndCallbackNotNull(Update newMessage) =>
         newMessage.Type == UpdateType.CallbackQuery && newMessage.CallbackQuery != null;

    private static async Task InitializeBotWithKeyboardButtons(Message message)
    {
        var buttonGenerator = new KeyboardButtonsGenerator();
        buttonGenerator.InitStartButtons();
        var startButtons = buttonGenerator.GetReplyKeyboardMarkup;

        var chatId = message.Chat.Id;
        if (ChatScreenHandlers.ContainsKey(chatId))
        {
            ChatScreenHandlers.Remove(chatId);
        }

        await Bot.SendTextMessageAsync(message.Chat.Id, "Start...",
            replyToMessageId: message.MessageId, replyMarkup: startButtons);
    }

    private static bool IsUpdateTypeMessageAndMessageNotNull(Update newMessage) =>
        newMessage.Type == UpdateType.Message && newMessage.Message != null;

    private static bool IsItStartMessage(Message message) =>
        message.Text?.ToLower() == "/start";
}

