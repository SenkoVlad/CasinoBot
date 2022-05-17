using Casino.Common.AppConstants;

namespace Casino.BLL.Models;

public class DepositModel
{
    public DepositCurrency Currency { get; set; }
    public int Amount { get; set; }

    public string GetDefaultChangeAmount()
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
            Amount, 
            Environment.NewLine,
            Currency.ToString());
    }

    public int GetHalvedAmount()
    {
        var halvedAmount = Amount / 2;
        switch (Currency)
        {
            case DepositCurrency.EURO:
                return halvedAmount >= AppConstants.EuroDefaultDepositIncreasingAmount
                    ? Amount 
                    : AppConstants.EuroDefaultDepositIncreasingAmount;
            case DepositCurrency.USD:
                return halvedAmount >= AppConstants.UsdDefaultDepositIncreasingAmount
                    ? Amount
                    : AppConstants.UsdDefaultDepositIncreasingAmount;
            case DepositCurrency.RUB:
                return halvedAmount >= AppConstants.RubDefaultDepositIncreasingAmount
                    ? Amount
                    : AppConstants.RubDefaultDepositIncreasingAmount;
            default:
                throw new Exception($"{nameof(GetHalvedAmount)} currency is out from default");
        }
    }

    public int GetIncreasedAmount()
    {
        int increasedAmount;
        switch (Currency)
        {
            case DepositCurrency.EURO:
                increasedAmount = Amount - AppConstants.EuroDefaultDepositIncreasingAmount;
                return increasedAmount >= AppConstants.EuroDefaultDepositIncreasingAmount
                    ? Amount
                    : AppConstants.EuroDefaultDepositIncreasingAmount;
            case DepositCurrency.USD:
                increasedAmount = Amount - AppConstants.UsdDefaultDepositIncreasingAmount;
                return increasedAmount >= AppConstants.UsdDefaultDepositIncreasingAmount
                    ? Amount
                    : AppConstants.UsdDefaultDepositIncreasingAmount;
            case DepositCurrency.RUB:
                increasedAmount = Amount - AppConstants.RubDefaultDepositIncreasingAmount;
                return increasedAmount >= AppConstants.RubDefaultDepositIncreasingAmount
                    ? Amount
                    : AppConstants.RubDefaultDepositIncreasingAmount;
            default:
                throw new Exception($"{nameof(GetHalvedAmount)} currency is out from default");
        }
    }

    public int GetDecreaseAmount()
    {
        int increasedAmount;
        switch (Currency)
        {
            case DepositCurrency.EURO:
                increasedAmount = Amount + AppConstants.EuroDefaultDepositIncreasingAmount;
                return increasedAmount >= AppConstants.EuroDefaultDepositIncreasingAmount
                    ? Amount
                    : AppConstants.EuroDefaultDepositIncreasingAmount;
            case DepositCurrency.USD:
                increasedAmount = Amount + AppConstants.UsdDefaultDepositIncreasingAmount;
                return increasedAmount >= AppConstants.UsdDefaultDepositIncreasingAmount
                    ? Amount
                    : AppConstants.UsdDefaultDepositIncreasingAmount;
            case DepositCurrency.RUB:
                increasedAmount = Amount + AppConstants.RubDefaultDepositIncreasingAmount;
                return increasedAmount >= AppConstants.RubDefaultDepositIncreasingAmount
                    ? Amount
                    : AppConstants.RubDefaultDepositIncreasingAmount;
            default:
                throw new Exception($"{nameof(GetHalvedAmount)} currency is out from default");
        }
    }
}