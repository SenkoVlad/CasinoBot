using Casino.Common.Enum;

namespace Casino.Common.Dtos;

public class CommandDto
{
    public Command Command { get; set; }
    public string? CommandParamJson { get; set; }
}