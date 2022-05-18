using Casino.Common.AppConstants;

namespace Casino.BLL.Models;

public class DepositModel
{
    public DepositCurrency Currency { get; set; }

    public int AmountCents { get; set; }

    public string GetDefaultChangeAmountString()
    {
        switch (Currency)
        {
            case DepositCurrency.EURO:
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

    public int IncreasedAmount()
    {
        return Currency switch
        {
            DepositCurrency.EURO => AmountCents + AppConstants.EuroDefaultDepositIncreasingAmount,
            DepositCurrency.USD => AmountCents + AppConstants.UsdDefaultDepositIncreasingAmount,
            DepositCurrency.RUB => AmountCents + AppConstants.RubDefaultDepositIncreasingAmount,
            _ => throw new Exception($"{nameof(IncreasedAmount)} currency is out from default")
        };
    }

    public int DecreaseAmount()
    {
        int decreasedAmount;
        switch (Currency)
        {
            case DepositCurrency.EURO:
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
                throw new Exception($"{nameof(DecreaseAmount)} currency is out from default");
        }
    }

    public int HalveAmount()
    {
        int decreasedAmount = AmountCents / 2;
        switch (Currency)
        {
            case DepositCurrency.EURO:
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
                throw new Exception($"{nameof(DecreaseAmount)} currency is out from default");
        }
    }
}