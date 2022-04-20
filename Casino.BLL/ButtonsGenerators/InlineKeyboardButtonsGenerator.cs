using Casino.Common.AppConstants;
using Casino.Common.Dtos;
using Casino.Common.Enum;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ButtonsGenerators;

public class InlineKeyboardButtonsGenerator
{
    private InlineKeyboardMarkup _inlineKeyboardMarkup;
    public InlineKeyboardMarkup GetInlineKeyboardMarkup => _inlineKeyboardMarkup;

    public void InitStartButtons()
    {
        var gamesButton = new InlineKeyboardButton(ButtonTextConstants.GamesButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetGamesButtonCommand
            })
        };
        var getInfoButton = new InlineKeyboardButton(ButtonTextConstants.GetInfoButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetInfoButtonCommand
            })
        };
        var balanceButton = new InlineKeyboardButton(ButtonTextConstants.BalanceButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetBalanceButtonCommand
            })
        };

        var buttonRows = new[]{ getInfoButton, balanceButton, gamesButton };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitGamesButtons()
    {
        var diceGameButton = new InlineKeyboardButton(ButtonTextConstants.DiceGameButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseDiceGameButtonCommand
            })
        };
        var russianRouletteGameButton = new InlineKeyboardButton(ButtonTextConstants.RussianRouletteGameButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseRussianRouletteGameButtonCommand
            })
        };
        var footballGameButton = new InlineKeyboardButton(ButtonTextConstants.FootballGameButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseFootballGameButtonCommand
            })
        };
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenuButtonCommand
            })
        };

        var buttonRows = new[] 
        {
            new[] { footballGameButton, russianRouletteGameButton, diceGameButton },
            new[] { backButton}
        };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitGetBalanceButtons(int payment)
    {
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenuButtonCommand
            })
        };
        var currentPayment = string.Concat(payment, ButtonTextConstants.BalancePaymentButtonText);
        var  balancePaymentAmountButton = new InlineKeyboardButton(currentPayment)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DoNothing
            })
        };
        var increaseBalanceButton = new InlineKeyboardButton(ButtonTextConstants.IncreaseBalancePaymentButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.IncreaseBalancePayment,
                CommandParam = payment + 1
            })
        };
        var decreaseBalanceButton = new InlineKeyboardButton(ButtonTextConstants.DecreaseBalancePaymentButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DecreaseBalancePayment,
                CommandParam = payment - 1
            })
        };
        var addBalanceButton = new InlineKeyboardButton(ButtonTextConstants.AddBalanceButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DepositPayment,
                CommandParam = payment
            })
        };

        var buttonRows = new[]
        {
            new [] { decreaseBalanceButton, balancePaymentAmountButton, increaseBalanceButton },
            new [] { addBalanceButton },
            new [] { backButton }
        };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitPlayFootballDemoButtons()
    {
        var hitBallButton = new InlineKeyboardButton(ButtonTextConstants.HitBallButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.HitBallButtonCommand
            })
        };
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseFootballGameButtonCommand
            })
        };

        var buttonRows = new[] { hitBallButton, backButton };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitDiceChooseBetButtons()
    {

        var onePointButton = new InlineKeyboardButton(ButtonTextConstants.DiceOnePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = 1
            })
        };
        var twoPointButton = new InlineKeyboardButton(ButtonTextConstants.DiceTwoPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = 2
            })
        };
        var threePointButton = new InlineKeyboardButton(ButtonTextConstants.DiceThreePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = 3
            })
        };
        var fourPointButton = new InlineKeyboardButton(ButtonTextConstants.DiceFourPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = 4
            })
        };
        var fivePointButton = new InlineKeyboardButton(ButtonTextConstants.DiceFivePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = 5
            })
        };
        var sixPointButton = new InlineKeyboardButton(ButtonTextConstants.DiceSixPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = 6
            })
        };
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetGamesButtonCommand
            })
        };

        var buttonRows = new[] 
        { 
            new[] 
            { 
                onePointButton, twoPointButton, threePointButton, fourPointButton, fivePointButton, sixPointButton
            },
            new []
            {
                backButton
            }
        };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitChooseFootballMode()
    {
        var playDemoFootballButton = new InlineKeyboardButton(ButtonTextConstants.DemoPlayFootBallButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DemoPlayFootballButtonCommand
            })
        };
        var playRealFootballButton = new InlineKeyboardButton(ButtonTextConstants.RealPlayFootBallButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.RealPlayFootballButtonCommand
            })
        };
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetGamesButtonCommand
            })
        };
        var buttonRows = new[]
        {
            new[]
            {
                playDemoFootballButton, playRealFootballButton,
            },
            new []
            {
                backButton
            }
        };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }
}