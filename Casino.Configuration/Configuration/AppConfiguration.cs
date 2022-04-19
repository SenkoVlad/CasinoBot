using Casino.Common.AppConstants;
using Microsoft.Extensions.Configuration;

namespace Casino.Configuration.Configuration;

public class AppConfiguration : IAppConfiguration
{
    private readonly IConfiguration _configuration;
    public AppConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string TelegramMessagesExchange => _configuration.GetSection(AppSettingConstants.TelegramMessagesExchangeSettingName).Value;
    public int RabbitPort => int.Parse(_configuration.GetSection(AppSettingConstants.RabbitPortSettingName).Value);
    public string RabbitHostName => _configuration.GetSection(AppSettingConstants.RabbitHostNameSettingName).Value;
    public string TelegramMessageBaseRoute => _configuration.GetSection(AppSettingConstants.RabbitTelegramMessageBaseRouteSettingName).Value;

    public string TelegramMessagesQueue =>
        _configuration.GetSection(AppSettingConstants.RabbitTelegramMessagesQueueSettingName).Value;
}