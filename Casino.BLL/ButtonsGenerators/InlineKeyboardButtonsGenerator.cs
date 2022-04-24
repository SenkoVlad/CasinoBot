using Casino.Common.AppConstants;
using Casino.Common.Dtos;
using Casino.Common.Enum;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ButtonsGenerators;

public class InlineKeyboardButtonsGenerator
{
    private InlineKeyboardMarkup _inlineKeyboardMarkup = null!;
    public InlineKeyboardMarkup GetInlineKeyboardMarkup => _inlineKeyboardMarkup;

    private readonly IStringLocalizer<Resources> _localizer;

    public InlineKeyboardButtonsGenerator(IStringLocalizer<Resources> localizer)
    {
        _localizer = localizer;
    }

    public void InitStartButtons()
    {
        var gamesButton = new InlineKeyboardButton(_localizer[Resources.GamesButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetGamesButtonCommand
            })
        };
        var settingButton = new InlineKeyboardButton(_localizer[Resources.SettingsButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.SettingsCommand
            })
        };
        var balanceButton = new InlineKeyboardButton(_localizer[Resources.BalanceButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetBalanceButtonCommand
            })
        };

        var buttonRows = new[]{ gamesButton, settingButton, balanceButton };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitGamesButtons()
    {
        var diceGameButton = new InlineKeyboardButton(_localizer[Resources.DiceGameButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseDiceGameButtonCommand
            })
        };
        var russianRouletteGameButton = new InlineKeyboardButton(_localizer[Resources.RussianRouletteGameButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseRussianRouletteGameButtonCommand
            })
        };
        var footballGameButton = new InlineKeyboardButton(_localizer[Resources.FootballGameButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseFootballGameButtonCommand
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
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
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenuButtonCommand
            })
        };
        var currentPayment = string.Concat(payment, ButtonConstants.DollarSignButtonText);
        var  balancePaymentAmountButton = new InlineKeyboardButton(currentPayment)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DoNothing
            })
        };
        var increaseBalanceButton = new InlineKeyboardButton(ButtonConstants.IncreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.IncreaseBalancePayment,
                CommandParamJson = JsonConvert.SerializeObject(payment + 1)
            })
        };
        var decreaseBalanceButton = new InlineKeyboardButton(ButtonConstants.DecreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DecreaseBalancePayment,
                CommandParamJson = JsonConvert.SerializeObject(payment - 1)
            })
        };
        var addBalanceButton = new InlineKeyboardButton(_localizer[Resources.AddBalanceButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DepositPayment,
                CommandParamJson = JsonConvert.SerializeObject(payment)
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

    public void InitPlayFootballDemoButtons(int userBet)
    {
        var hitBallButton = new InlineKeyboardButton(_localizer[Resources.HitBallButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.HitBallButtonCommand,
                CommandParamJson = JsonConvert.SerializeObject(userBet)
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseFootballGameButtonCommand
            })
        };
        var currentUserBet = string.Concat(userBet, ButtonConstants.DollarSignButtonText);
        var currentUserBetButton = new InlineKeyboardButton(currentUserBet)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DoNothing
            })
        };
        var increaseBetButton = new InlineKeyboardButton(ButtonConstants.IncreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.IncreaseFootballBetPayment,
                CommandParamJson = JsonConvert.SerializeObject(userBet + 1)
            })
        };
        var decreaseBetButton = new InlineKeyboardButton(ButtonConstants.DecreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DecreaseFootballBetPayment,
                CommandParamJson = JsonConvert.SerializeObject(userBet - 1)
            })
        };
        var buttonRows = new[]
        {
            new[] { decreaseBetButton, currentUserBetButton, increaseBetButton},
            new[] { hitBallButton, backButton }
        };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitDiceChooseBetButtons(int userBet)
    {
        var onePointButton = new InlineKeyboardButton(ButtonConstants.DiceOnePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParamJson = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 1,
                    UserBet = userBet
                })
            })
        };
        var twoPointButton = new InlineKeyboardButton(ButtonConstants.DiceTwoPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParamJson = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 2,
                    UserBet = userBet
                })
            })
        };
        var threePointButton = new InlineKeyboardButton(ButtonConstants.DiceThreePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParamJson = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 3,
                    UserBet = userBet
                })
            })
        };
        var fourPointButton = new InlineKeyboardButton(ButtonConstants.DiceFourPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParamJson = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 4,
                    UserBet = userBet
                })
            })
        };
        var fivePointButton = new InlineKeyboardButton(ButtonConstants.DiceFivePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParamJson = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 5,
                    UserBet = userBet
                })
            })
        };
        var sixPointButton = new InlineKeyboardButton(ButtonConstants.DiceSixPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParamJson = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 6,
                    UserBet = userBet
                })
            })
        };
        var currentUserBet = string.Concat(userBet, ButtonConstants.DollarSignButtonText);
        var currentUserBetButton = new InlineKeyboardButton(currentUserBet)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DoNothing
            })
        };
        var increaseBetButton = new InlineKeyboardButton(ButtonConstants.IncreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.IncreaseDiceBetPayment,
                CommandParamJson = JsonConvert.SerializeObject(userBet + 1)
            })
        };
        var decreaseBetButton = new InlineKeyboardButton(ButtonConstants.DecreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DecreaseDiceBetPayment,
                CommandParamJson = JsonConvert.SerializeObject(userBet - 1)
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetGamesButtonCommand
            })
        };

        var buttonRows = new[] 
        {
            new []
            {
                decreaseBetButton, currentUserBetButton, increaseBetButton
            },
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
        var playDemoFootballButton = new InlineKeyboardButton(_localizer[Resources.DemoPlayFootBallButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DemoPlayFootballButtonCommand
            })
        };
        var playRealFootballButton = new InlineKeyboardButton(_localizer[Resources.RealPlayFootBallButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.RealPlayFootballButtonCommand
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
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

    public void InitSettingsButtons()
    {
        var en = "en-US";
        var ru = "ru-RU";

        var switchToEnglishButton = new InlineKeyboardButton(en)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.SwitchLanguageCommand,
                CommandParamJson = en
            })
        };
        var switchToRussianButton = new InlineKeyboardButton(ru)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.SwitchLanguageCommand,
                CommandParamJson = ru
            })
        };
        var a = _localizer[Resources.BackButtonText];
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenuButtonCommand
            })
        };
        var buttonRows = new[]
        {
            new[]
            {
                switchToEnglishButton, switchToRussianButton
            },
            new []
            {
                backButton
            }
        };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }
}