namespace Casino.BLL.Models;

public class PaymentModel
{
    public long Id { get; set; }
    public long ChatId { get; set; }
    public string Currency { get; set; }
    public float TotalAmount { get; set; }
}