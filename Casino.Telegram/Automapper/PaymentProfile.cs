using AutoMapper;
using Casino.BLL.Models;
using Telegram.Bot.Types.Payments;

namespace Casino.Telegram.Automapper;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<PreCheckoutQuery, PaymentModel>()
            .ForMember(p => p.ChatId,
                m => m.MapFrom(t => t.From.Id))
            .ForMember(p => p.TotalAmount,
                m => m.MapFrom(t => t.TotalAmount / 100));
    }
}