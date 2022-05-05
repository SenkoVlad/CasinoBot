using Casino.Common.Enum;

namespace Casino.BLL.Models;

public class WithdrawModel
{
    public WithdrawMethod Method { get; set; }
    public int Amount { get; set; }
}