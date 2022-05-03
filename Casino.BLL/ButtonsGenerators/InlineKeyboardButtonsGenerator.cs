﻿using Casino.BLL.Models;
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
                Command = Command.GetGames
            })
        };
        var settingButton = new InlineKeyboardButton(_localizer[Resources.SettingsButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.Settings
            })
        };
        var balanceButton = new InlineKeyboardButton(_localizer[Resources.BalanceButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetBalance
            })
        };

        var buttonRows = new[]{ gamesButton, settingButton, balanceButton };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = GetBalanceMessage(chat);
    }

    private string GetBalanceMessage(ChatModel chat)
    {
        return string.Concat(
            _localizer[Resources.GetMyBalanceResource, chat.Balance],
            Environment.NewLine,
            _localizer[Resources.GetMyDemoBalanceResource, chat.DemoBalance],
            Environment.NewLine,
            _localizer[Resources.ChooseActionResource]);
    }


    public void InitGamesButtons()
    {
        var diceGameButton = new InlineKeyboardButton(_localizer[Resources.DiceGameButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseDice
            })
        };
        var footballGameButton = new InlineKeyboardButton(_localizer[Resources.FootballGameButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseFootball
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenuButton
            })
        };

        var buttonRows = new[] 
        {
            new[] { footballGameButton, diceGameButton },
            new[] { backButton}
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.ChooseActionResource];
    }

    public void InitGetBalanceButtons(int payment, ChatModel chatModel)
    {
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenuButton
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
                Command = Command.IncreaseBalance,
                Param = JsonConvert.SerializeObject(payment + 1)
            })
        };
        var decreaseBalanceButton = new InlineKeyboardButton(ButtonConstants.DecreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DecreaseBalance,
                Param = JsonConvert.SerializeObject(payment - 1)
            })
        };
        var addBalanceButton = new InlineKeyboardButton(_localizer[Resources.AddBalanceButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DepositPayment,
                Param = JsonConvert.SerializeObject(payment)
            })
        };

        var buttonRows = new[]
        {
            new [] { decreaseBalanceButton, balancePaymentAmountButton, increaseBalanceButton },
            new [] { addBalanceButton },
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.GetMyBalanceResource, chatModel.Balance];
    }

    public void InitPlayFootballButtons(GameModel gameModel)
    {
        var hitBallButton = new InlineKeyboardButton(_localizer[Resources.HitBallButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.HitBall,
                Param = JsonConvert.SerializeObject(new FootballGameParamDto
                {
                    Bet = gameModel.UserBet,
                    IsDemo = gameModel.IsDemoPlay
                })
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseFootball
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
                Command = Command.IncreaseFootballBet,
                Param = JsonConvert.SerializeObject(new FootballGameParamDto
                {
                    Bet = gameModel.UserBet + 1,
                    IsDemo = gameModel.IsDemoPlay
                })
            })
        };
        var decreaseBetButton = new InlineKeyboardButton(ButtonConstants.DecreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DecreaseFootballBet,
                Param = JsonConvert.SerializeObject(new FootballGameParamDto
                {
                    Bet = gameModel.UserBet - 1,
                    IsDemo = gameModel.IsDemoPlay
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
            ? _localizer[Resources.GetMyDemoBalanceResource, gameModel.Chat.DemoBalance]
            : _localizer[Resources.GetMyBalanceResource, gameModel.Chat.Balance];
    }

    public void InitDiceChooseBetButtons(GameModel gameModel)
    {
        var onePointButton = new InlineKeyboardButton(ButtonConstants.DiceOnePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 1,
                    Bet = gameModel.UserBet,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var twoPointButton = new InlineKeyboardButton(ButtonConstants.DiceTwoPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 2,
                    Bet = gameModel.UserBet,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var threePointButton = new InlineKeyboardButton(ButtonConstants.DiceThreePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 3,
                    Bet = gameModel.UserBet,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var fourPointButton = new InlineKeyboardButton(ButtonConstants.DiceFourPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 4,
                    Bet = gameModel.UserBet,
                    IsDemo =  Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var fivePointButton = new InlineKeyboardButton(ButtonConstants.DiceFivePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 5,
                    Bet = gameModel.UserBet,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var sixPointButton = new InlineKeyboardButton(ButtonConstants.DiceSixPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 6,
                    Bet = gameModel.UserBet,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
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
            CallbackData =  JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.IncreaseDiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 1,
                    Bet = gameModel.UserBet + 1,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var decreaseBetButton = new InlineKeyboardButton(ButtonConstants.DecreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DecreaseDiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 1,
                    Bet = gameModel.UserBet - 1,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetGames
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
            ? _localizer[Resources.GetMyDemoBalanceResource, gameModel.Chat.DemoBalance]
            : _localizer[Resources.GetMyBalanceResource, gameModel.Chat.Balance];
    }

    public void InitChooseFootballMode()
    {
        var playDemoFootballButton = new InlineKeyboardButton(_localizer[Resources.DemoPlayFootBallButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.PlayFootball,
                Param = true.ToString()
            })
        };
        var playRealFootballButton = new InlineKeyboardButton(_localizer[Resources.RealPlayFootBallButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.PlayFootball,
                Param = false.ToString()
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetGames
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
        ReplyText = _localizer[Resources.ChooseGameModeResource];
    }

    public void InitChooseDiceMode()
    {
        var playDemoDiceButton = new InlineKeyboardButton(_localizer[Resources.DemoPlayDiceButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.PlayDice,
                Param = true.ToString()
            })
        };
        var playRealDiceButton = new InlineKeyboardButton(_localizer[Resources.RealPlayDiceButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.PlayDice,
                Param = false.ToString()
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetGames
            })
        };
        var buttonRows = new[]
        {
            new[]
            {
                playDemoDiceButton, playRealDiceButton,
            },
            new []
            {
                backButton
            }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.ChooseGameModeResource];
    }

    public void InitSettingsButtons(string currentLanguage)
    {
        var en = "en-US";
        var ru = "ru-RU";

        var switchToEnglishButton = new InlineKeyboardButton(en)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.SwitchLanguage,
                Param = en
            })
        };
        var switchToRussianButton = new InlineKeyboardButton(ru)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.SwitchLanguage,
                Param = ru
            })
        };
        var a = _localizer[Resources.BackButtonText];
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenuButton
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