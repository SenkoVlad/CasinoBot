namespace Casino.Configuration.Configuration;

public interface IAppConfiguration
{
    public string TelegramMessagesExchange { get; }
    public int RabbitPort { get; }
    public string RabbitHostName { get; }
    public string TelegramMessageBaseRoute { get; }
    public string TelegramMessagesQueue { get; }
}