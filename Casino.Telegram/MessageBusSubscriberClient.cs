using System.Text;
using Casino.BLL.ScreenHandlers.Implementation;
using Casino.Common.Dtos;
using Casino.Common.Enum;
using Casino.Configuration.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Telegram.Bot;

namespace Casino.TelegramConsumer;

public class MessageBusSubscriberClient : IMessageBusSubscriberClient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _rabbitMqConnection;
    private readonly IModel _rabbitMqChannel;
    private readonly string _rabbitMqQueueName;
    private readonly IAppConfiguration _appConfiguration;

    public MessageBusSubscriberClient(IServiceProvider serviceProvider, 
        IAppConfiguration appConfiguration)
    {
        _serviceProvider = serviceProvider;
        _appConfiguration = appConfiguration;
        var messageBusConnectionFactory = new ConnectionFactory
        {
            HostName = _appConfiguration.RabbitHostName,
            Port = _appConfiguration.RabbitPort
        };

        try
        {
            _rabbitMqConnection = messageBusConnectionFactory.CreateConnection();
            _rabbitMqChannel = _rabbitMqConnection.CreateModel();
            _rabbitMqChannel.ExchangeDeclare(exchange: _appConfiguration.TelegramMessagesExchange, type: ExchangeType.Fanout, durable:true);
            _rabbitMqQueueName = _rabbitMqChannel.QueueDeclare(_appConfiguration.TelegramMessagesQueue, true, exclusive:false, autoDelete:false).QueueName;
            _rabbitMqChannel.QueueBind(queue: _rabbitMqQueueName,
                exchange: _appConfiguration.TelegramMessagesExchange,
                routingKey: _appConfiguration.TelegramMessageBaseRoute);
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
                switch (telegramMessage.Message.MessageType)
                {
                    case MessageType.Start:
                        await ProcessStartMessageAsync(telegramMessage, telegramBotClient);
                        break;
                    case MessageType.Callback:
                        await ProcessButtonClickAsync(telegramMessage, telegramBotClient);
                        break;
                    case MessageType.UserMessage:
                        await telegramBotClient.DeleteMessageAsync(telegramMessage.Message.ChatId,
                            telegramMessage.Message.MessageId);
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
        var inlineButtonPushHandler = new InlineScreenHandler(
            telegramMessage.Message.ChatId,
            telegramMessage.Message.MessageId,
            telegramBotClient,
            _serviceProvider);
        await inlineButtonPushHandler.PushButtonAsync(telegramMessage.Message.CommandDto);
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

    private async Task ProcessStartMessageAsync(TelegramMessageDto message, ITelegramBotClient telegramBotClient)
    {
        var inlineButtonPushHandler = new InlineScreenHandler(message.Message.ChatId, message.Message.MessageId, 
            telegramBotClient, _serviceProvider);
        await inlineButtonPushHandler.PushButtonAsync(new CommandDto
        {
            Command = Command.StartCommand
        });
    }
}