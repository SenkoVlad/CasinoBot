using AutoMapper;
using Casino.BLL.Models;
using Casino.Common.Dtos;
using Casino.DAL.Models;
using Telegram.Bot.Types.Payments;
using Currency = Casino.Common.Enum.Currency;

namespace Casino.BLL.AutoMapper;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<PaymentModel, Payment>()
            .ForMember(p => p.Id, m => m.Ignore())
            .ForMember(p => p.PaymentProviderId,
                m => m.MapFrom(c => c.PaymentProviderId))
            .ForMember(p => p.CurrencyId,
                m => m.MapFrom(c => (int) Enum.Parse(typeof(Currency), c.Currency)));
        CreateMap<DepositDto, DepositModel>();
        CreateMap<SuccessfulPayment, PaymentModel>()
            .ForMember(p => p.ChatId, m => m.MapFrom(s => long.Parse(s.InvoicePayload)))
            .ForMember(p => p.Currency, m => m.MapFrom(s => s.Currency))
            .ForMember(p => p.TotalAmount, m => m.MapFrom(s => s.TotalAmount / 100))
            .ForMember(p => p.PaymentProviderId, m => m.MapFrom(s => s.ProviderPaymentChargeId));

    }
}