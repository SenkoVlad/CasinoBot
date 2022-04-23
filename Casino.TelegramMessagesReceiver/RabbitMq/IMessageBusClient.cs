using Casino.Common.Dtos;

namespace Casino.TelegramSender.RabbitMq;

public interface IMessageBusClient
{
    public void PublishMessage(TelegramMessageDto telegramMessageDto);
}