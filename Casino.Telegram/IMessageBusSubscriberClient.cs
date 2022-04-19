using Telegram.Bot;

namespace Casino.TelegramConsumer;

public interface IMessageBusSubscriberClient
{
    Task StartListening(ITelegramBotClient telegramBotClient);
}