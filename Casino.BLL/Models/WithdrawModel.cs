using Casino.Common.Enum;

namespace Casino.BLL.Models;

public class WithdrawModel
{
    public Currency Method { get; set; }
    public int Amount { get; set; }
}