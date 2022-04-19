using Casino.Common.Enum;

namespace Casino.Common.Dtos;

public class MessageDto
{
    public int MessageId { get; set; }
    public long ChatId { get; set; }
    public string? Text { get; set; }
    public CommandDto CommandDto { get; set; }
    public MessageType MessageType { get; set; }
}