using Telegram.Bot;

namespace Casino.TelegramConsumer.RabbitMq;

public interface IMessageBusSubscriberClient
{
    Task StartListening(ITelegramBotClient telegramBotClient);
}