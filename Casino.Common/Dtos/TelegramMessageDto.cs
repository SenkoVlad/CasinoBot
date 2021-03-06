using Casino.Common.Enum;

namespace Casino.Common.Dtos;

public class TelegramMessageDto
{
    public int MessageId { get; set; }
    public long ChatId { get; set; }
    public string? Text { get; set; }
    public CommandDto CommandDto { get; set; } = null!;
    public MessageType MessageType { get; set; }
    public string CallbackQueryId { get; set; }
}