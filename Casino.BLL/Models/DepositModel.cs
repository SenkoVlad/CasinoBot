using Casino.Common.AppConstants;
using Casino.Common.Enum;

namespace Casino.BLL.Models;

public class DepositModel
{
    public DepositCurrency Currency { get; set; }

    public int AmountCents { get; set; }

    public string GetDefaultChangeAmountString()
    {
        switch (Currency)
        {
            case DepositCurrency.EUR:
                return string.Concat(AppConstants.EuroDefaultDepositIncreasingAmount, " ", Currency.ToString());
            case DepositCurrency.USD:
                return string.Concat(AppConstants.UsdDefaultDepositIncreasingAmount, " ", Currency.ToString());
            case DepositCurrency.RUB:
                return string.Concat(AppConstants.RubDefaultDepositIncreasingAmount, " ", Currency.ToString());
            default:
                return string.Concat(AppConstants.DefaultDepositAmount, " ", AppConstants.DefaultDepositCurrency);
        }
    }

    public override string ToString()
    {
        return string.Concat(
            AmountCents, 
            Environment.NewLine,
            Currency.ToString());
    }

    public int GetIncreasedAmount()
    {
        return Currency switch
        {
            DepositCurrency.EUR => AmountCents + AppConstants.EuroDefaultDepositIncreasingAmount,
            DepositCurrency.USD => AmountCents + AppConstants.UsdDefaultDepositIncreasingAmount,
            DepositCurrency.RUB => AmountCents + AppConstants.RubDefaultDepositIncreasingAmount,
            _ => throw new Exception($"{nameof(GetIncreasedAmount)} currency is out from default")
        };
    }

    public int GetDecreaseAmount()
    {
        int decreasedAmount;
        switch (Currency)
        {
            case DepositCurrency.EUR:
                decreasedAmount = AmountCents - AppConstants.EuroDefaultDepositIncreasingAmount;
                return decreasedAmount >= AppConstants.EuroDefaultDepositIncreasingAmount
                    ? decreasedAmount
                    : AppConstants.EuroDefaultDepositIncreasingAmount;
            case DepositCurrency.USD:
                decreasedAmount = AmountCents - AppConstants.UsdDefaultDepositIncreasingAmount;
                return decreasedAmount >= AppConstants.UsdDefaultDepositIncreasingAmount
                    ? decreasedAmount
                    : AppConstants.UsdDefaultDepositIncreasingAmount;
            case DepositCurrency.RUB:
                decreasedAmount = AmountCents - AppConstants.RubDefaultDepositIncreasingAmount;
                return decreasedAmount >= AppConstants.RubDefaultDepositIncreasingAmount
                    ? decreasedAmount
                    : AppConstants.RubDefaultDepositIncreasingAmount;
            default:
                throw new Exception($"{nameof(GetDecreaseAmount)} currency is out from default");
        }
    }

    public int GetHalveAmount()
    {
        int decreasedAmount = AmountCents / 2;
        switch (Currency)
        {
            case DepositCurrency.EUR:
                return decreasedAmount >= AppConstants.EuroDefaultDepositIncreasingAmount
                    ? decreasedAmount
                    : AppConstants.EuroDefaultDepositIncreasingAmount;
            case DepositCurrency.USD:
                return decreasedAmount >= AppConstants.UsdDefaultDepositIncreasingAmount
                    ? decreasedAmount
                    : AppConstants.UsdDefaultDepositIncreasingAmount;
            case DepositCurrency.RUB:
                return decreasedAmount >= AppConstants.RubDefaultDepositIncreasingAmount
                    ? decreasedAmount
                    : AppConstants.RubDefaultDepositIncreasingAmount;
            default:
                throw new Exception($"{nameof(GetDecreaseAmount)} currency is out from default");
        }
    }
}