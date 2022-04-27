using Casino.BLL.Models;
using Casino.Common.AppConstants;
using Casino.Common.Dtos;
using Casino.Common.Enum;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ButtonsGenerators;

public class InlineKeyboardButtonsGenerator
{
    private readonly IStringLocalizer<Resources> _localizer;
    
    public InlineKeyboardMarkup GetInlineKeyboardMarkup { get; private set; }
    public string ReplyText { get; private set; }

    public InlineKeyboardButtonsGenerator(IStringLocalizer<Resources> localizer)
    {
        _localizer = localizer;
    }

    public void InitStartButtons(ChatModel chat)
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
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = GetBalanceMessage(chat);
    }

    private string GetBalanceMessage(ChatModel chat)
    {
        return string.Concat(
            _localizer[Resources.GetMyBalanceMessageText, chat.Balance],
            Environment.NewLine,
            _localizer[Resources.GetMyDemoBalanceMessageText, chat.DemoBalance],
            Environment.NewLine,
            _localizer[Resources.ChooseActionMessageText]);
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
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.ChooseActionMessageText];
    }

    public void InitGetBalanceButtons(int payment, string currentBalance)
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
                CommandParam = JsonConvert.SerializeObject(payment + 1)
            })
        };
        var decreaseBalanceButton = new InlineKeyboardButton(ButtonConstants.DecreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DecreaseBalancePayment,
                CommandParam = JsonConvert.SerializeObject(payment - 1)
            })
        };
        var addBalanceButton = new InlineKeyboardButton(_localizer[Resources.AddBalanceButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DepositPayment,
                CommandParam = JsonConvert.SerializeObject(payment)
            })
        };

        var buttonRows = new[]
        {
            new [] { decreaseBalanceButton, balancePaymentAmountButton, increaseBalanceButton },
            new [] { addBalanceButton },
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.GetMyBalanceMessageText, currentBalance];
    }

    public void InitPlayFootballButtons(GameModel gameModel)
    {
        var hitBallButton = new InlineKeyboardButton(_localizer[Resources.HitBallButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.HitBallButtonCommand,
                CommandParam = JsonConvert.SerializeObject(new FootballGameParamDto
                {
                    UserBet = gameModel.UserBet,
                    IsDemoPlay = gameModel.IsDemoPlay
                })
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseFootballGameButtonCommand
            })
        };
        var currentUserBet = string.Concat(gameModel.UserBet, ButtonConstants.DollarSignButtonText);
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
                CommandParam = JsonConvert.SerializeObject(new FootballGameParamDto
                {
                    UserBet = gameModel.UserBet + 1,
                    IsDemoPlay = gameModel.IsDemoPlay
                })
            })
        };
        var decreaseBetButton = new InlineKeyboardButton(ButtonConstants.DecreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DecreaseFootballBetPayment,
                CommandParam = JsonConvert.SerializeObject(new FootballGameParamDto
                {
                    UserBet = gameModel.UserBet - 1,
                    IsDemoPlay = gameModel.IsDemoPlay
                })
            })
        };
        var buttonRows = new[]
        {
            new[] { decreaseBetButton, currentUserBetButton, increaseBetButton},
            new[] { hitBallButton, backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = gameModel.IsDemoPlay
            ? _localizer[Resources.GetMyDemoBalanceMessageText, gameModel.Chat.DemoBalance]
            : _localizer[Resources.GetMyBalanceMessageText, gameModel.Chat.Balance];
    }

    public void InitDiceChooseBetButtons(GameModel gameModel)
    {
        var onePointButton = new InlineKeyboardButton(ButtonConstants.DiceOnePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 1,
                    UserBet = gameModel.UserBet,
                    IsDemoPlay = gameModel.IsDemoPlay
                })
            })
        };
        var twoPointButton = new InlineKeyboardButton(ButtonConstants.DiceTwoPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 2,
                    UserBet = gameModel.UserBet,
                    IsDemoPlay = gameModel.IsDemoPlay
                })
            })
        };
        var threePointButton = new InlineKeyboardButton(ButtonConstants.DiceThreePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 3,
                    UserBet = gameModel.UserBet,
                    IsDemoPlay = gameModel.IsDemoPlay
                })
            })
        };
        var fourPointButton = new InlineKeyboardButton(ButtonConstants.DiceFourPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 4,
                    UserBet = gameModel.UserBet,
                    IsDemoPlay = gameModel.IsDemoPlay
                })
            })
        };
        var fivePointButton = new InlineKeyboardButton(ButtonConstants.DiceFivePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 5,
                    UserBet = gameModel.UserBet,
                    IsDemoPlay = gameModel.IsDemoPlay
                })
            })
        };
        var sixPointButton = new InlineKeyboardButton(ButtonConstants.DiceSixPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.MakeBetButtonCommand,
                CommandParam = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 6,
                    UserBet = gameModel.UserBet,
                    IsDemoPlay = gameModel.IsDemoPlay
                })
            })
        };
        var currentUserBet = string.Concat(gameModel, ButtonConstants.DollarSignButtonText);
        var currentUserBetButton = new InlineKeyboardButton(currentUserBet)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DoNothing
            })
        };
        var increaseBetButton = new InlineKeyboardButton(ButtonConstants.IncreaseAmountButtonText)
        {
            CallbackData =  JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.IncreaseDiceBetPayment,
                CommandParam = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 1,
                    UserBet = gameModel.UserBet + 1,
                    IsDemoPlay = gameModel.IsDemoPlay
                })
            })
        };
        var decreaseBetButton = new InlineKeyboardButton(ButtonConstants.DecreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DecreaseDiceBetPayment,
                CommandParam = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 1,
                    UserBet = gameModel.UserBet - 1,
                    IsDemoPlay = gameModel.IsDemoPlay
                })
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
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = gameModel.IsDemoPlay
            ? _localizer[Resources.GetMyDemoBalanceMessageText, gameModel.Chat.DemoBalance]
            : _localizer[Resources.GetMyBalanceMessageText, gameModel.Chat.Balance];
    }

    public void InitChooseFootballMode()
    {
        var playDemoFootballButton = new InlineKeyboardButton(_localizer[Resources.DemoPlayFootBallButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.PlatFootballButtonCommand,
                CommandParam = true.ToString()
            })
        };
        var playRealFootballButton = new InlineKeyboardButton(_localizer[Resources.RealPlayFootBallButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.PlatFootballButtonCommand,
                CommandParam = false.ToString()
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
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.ChooseGameModeMessageText];
    }

    public void InitSettingsButtons(string currentLanguage)
    {
        var en = "en-US";
        var ru = "ru-RU";

        var switchToEnglishButton = new InlineKeyboardButton(en)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.SwitchLanguageCommand,
                CommandParam = en
            })
        };
        var switchToRussianButton = new InlineKeyboardButton(ru)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.SwitchLanguageCommand,
                CommandParam = ru
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
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = string.Concat(
            _localizer[Resources.CurrentLanguageButtonText],
            currentLanguage);
    }
}