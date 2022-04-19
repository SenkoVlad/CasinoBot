using Casino.Common.Dtos;

namespace Casino.TelegramSender.RabbitMq;

public interface IMessageBusClient
{
    public void PublishPlatform(TelegramMessageDto telegramMessageDto);
}