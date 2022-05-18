using System.ComponentModel.DataAnnotations;

namespace Casino.DAL.Models;

public class Payment
{
    [Key] 
    public long Id { get; set; }
    
    public string PaymentProviderId { get; set; }
    public long ChatId { get; set; }
    public int CurrencyId { get; set; }
    public float TotalAmount { get; set; }
}