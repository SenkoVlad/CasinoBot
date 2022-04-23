using System.Text;
using Casino.BLL.ClickHandlers.Implementation;
using Casino.Common.Dtos;
using Casino.Common.Enum;
using Casino.Configuration.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Telegram.Bot;

namespace Casino.TelegramConsumer.RabbitMq;

public class MessageBusSubscriberClient : IMessageBusSubscriberClient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _rabbitMqConnection;
    private readonly IModel _rabbitMqChannel;
    private readonly string _rabbitMqQueueName;

    public MessageBusSubscriberClient(IServiceProvider serviceProvider, 
        IAppConfiguration appConfiguration)
    {
        _serviceProvider = serviceProvider;
        var messageBusConnectionFactory = new ConnectionFactory
        {
            HostName = appConfiguration.RabbitHostName,
            Port = appConfiguration.RabbitPort
        };

        try
        {
            _rabbitMqConnection = messageBusConnectionFactory.CreateConnection();
            _rabbitMqChannel = _rabbitMqConnection.CreateModel();
            _rabbitMqChannel.ExchangeDeclare(exchange: appConfiguration.TelegramMessagesExchange, type: ExchangeType.Fanout, durable:true);
            _rabbitMqQueueName = _rabbitMqChannel.QueueDeclare(appConfiguration.TelegramMessagesQueue, true, exclusive:false, autoDelete:false).QueueName;
            _rabbitMqChannel.QueueBind(queue: _rabbitMqQueueName,
                exchange: appConfiguration.TelegramMessagesExchange,
                routingKey: appConfiguration.TelegramMessageBaseRoute);
            _rabbitMqConnection.ConnectionShutdown += RabbitMqConnectionOnConnectionShutdown;
        }
        catch (Exception)
        {
            throw new Exception("Something wrong with Rabbit");
        }
    }

    private void RabbitMqConnectionOnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Dispose();
    }

    public Task StartListening(ITelegramBotClient telegramBotClient)
    {
        var consumer = new EventingBasicConsumer(_rabbitMqChannel);
        consumer.Received +=  (_, args) =>
        {
            var body = args.Body;
            var telegramMessageDtoJson = Encoding.UTF8.GetString(body.ToArray());
            var telegramMessage = JsonConvert.DeserializeObject<TelegramMessageDto>(telegramMessageDtoJson);
            ProcessMessage(telegramMessage, telegramBotClient);
            Console.WriteLine($"{DateTime.Now}: Message processed");
        };
        _rabbitMqChannel.BasicConsume(queue: _rabbitMqQueueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    private void ProcessMessage(TelegramMessageDto telegramMessage, ITelegramBotClient telegramBotClient)
    {
        Task.Run(async () =>
        {
            try
            {
                switch (telegramMessage.MessageType)
                {
                    case MessageType.Start:
                    case MessageType.Callback:
                        await ProcessButtonClickAsync(telegramMessage, telegramBotClient);
                        break;
                    case MessageType.UserMessage:
                        await telegramBotClient.DeleteMessageAsync(telegramMessage.ChatId,
                            telegramMessage.MessageId);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
    }

    private async Task ProcessButtonClickAsync(TelegramMessageDto telegramMessage, ITelegramBotClient telegramBotClient)
    {
        var inlineButtonPushHandler = new ButtonClickHandler(
            telegramMessage,
            telegramBotClient,
            _serviceProvider);
        await inlineButtonPushHandler.PushButtonAsync();
    }

    public void Dispose()
    {
        if (_rabbitMqChannel.IsOpen)
        {
            _rabbitMqChannel.Close();
            _rabbitMqConnection.Close();

            _rabbitMqChannel.Dispose();
            _rabbitMqConnection.Dispose();
        }
    }
}