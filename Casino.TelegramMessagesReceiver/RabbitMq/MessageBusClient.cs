using System.Text;
using System.Text.Json;
using Casino.Common.Dtos;
using Casino.Configuration.Configuration;
using RabbitMQ.Client;

namespace Casino.TelegramSender.RabbitMq;

public class MessageBusClient : IMessageBusClient
{
    private readonly IAppConfiguration _appConfiguration;
    private readonly IConnection _rabbitMqConnection;
    private readonly IModel _rabbitMqChannel;

    public MessageBusClient(IAppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
        var messageBusConnectionFactory = new ConnectionFactory
        {
            HostName = appConfiguration.RabbitHostName,
            Port = appConfiguration.RabbitPort
        };

        try
        {
            _rabbitMqConnection = messageBusConnectionFactory.CreateConnection();
            _rabbitMqChannel = _rabbitMqConnection.CreateModel();
            _rabbitMqChannel.ExchangeDeclare(exchange: appConfiguration.TelegramMessagesExchange, 
                type: ExchangeType.Fanout,
                durable:true);
            _rabbitMqConnection.ConnectionShutdown += RabbitMqConnectionOnConnectionShutdown;
        }
        catch (Exception)
        {
            _rabbitMqConnection?.Dispose();
        }
    }

    private void RabbitMqConnectionOnConnectionShutdown(object? sender, ShutdownEventArgs e)
    { 
        Dispose();
    }
    
    private void SendMessageToRabbitMq(string messageJson)
    {
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        if (_rabbitMqChannel is {IsOpen: true})
        {
            _rabbitMqChannel.BasicPublish(exchange: _appConfiguration.TelegramMessagesExchange,
                routingKey: _appConfiguration.TelegramMessageBaseRoute,
                basicProperties: null,
                body:messageBytes);
        }

        Console.WriteLine($"{DateTime.Now}: Message sent");
    }

    private void Dispose()
    {
        if (_rabbitMqChannel.IsOpen)
        {
            _rabbitMqChannel.Close();
            _rabbitMqConnection.Close();
        }
    }

    public void PublishPlatform(TelegramMessageDto telegramMessageDto)
    {
        var messageJson = JsonSerializer.Serialize(telegramMessageDto);
        if (_rabbitMqConnection is { IsOpen: true })
        {
            SendMessageToRabbitMq(messageJson);
        }
    }
}