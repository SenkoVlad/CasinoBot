using AutoMapper;
using Casino.BLL.Models;
using Casino.Common.Dtos;
using Casino.DAL.Models;
using Currency = Casino.Common.Enum.Currency;

namespace Casino.BLL.AutoMapper;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<PaymentModel, Payment>()
            .ForMember(p => p.Id, m => m.Ignore())
            .ForMember(p => p.PaymentId,
                m => m.MapFrom(c => c.Id))
            .ForMember(p => p.CurrencyId,
                m => m.MapFrom(c => (int) Enum.Parse(typeof(Currency), c.Currency)));
        CreateMap<DepositDto, DepositModel>();
    }
}