﻿using Casino.BLL.Models;
using Casino.Common.AppConstants;
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
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.GetGamesButtonCommand
            })
        };
        var getInfoButton = new InlineKeyboardButton(ButtonTextConstants.GetInfoButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.GetInfoButtonCommand
            })
        };
        var balanceButton = new InlineKeyboardButton(ButtonTextConstants.BalanceButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.GetBalanceButtonCommand
            })
        };

        var buttonRows = new[]{ getInfoButton, balanceButton, gamesButton };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitGamesButtons()
    {
        var diceGameButton = new InlineKeyboardButton(ButtonTextConstants.DiceGameButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.ChooseDiceGameButtonCommand
            })
        };
        var russianRouletteGameButton = new InlineKeyboardButton(ButtonTextConstants.RussianRouletteGameButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.ChooseRussianRouletteGameButtonCommand
            })
        };
        var footballGameButton = new InlineKeyboardButton(ButtonTextConstants.FootballGameButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.ChooseFootballGameButtonCommand
            })
        };
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.ToMenuButtonCommand
            })
        };

        var buttonRows = new[] 
        {
            new[] { footballGameButton, russianRouletteGameButton, diceGameButton },
            new[] { backButton}
        };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitGetBalanceButtons()
    {
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.ToMenuButtonCommand
            })
        };

        var buttonRows = new[] { backButton };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitPlayFootballDemoButtons()
    {
        var hitBallButton = new InlineKeyboardButton(ButtonTextConstants.HitBallButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.HitBallButtonCommand
            })
        };
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.ChooseFootballGameButtonCommand
            })
        };

        var buttonRows = new[] { hitBallButton, backButton };
        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void InitDiceChooseBetButtons()
    {

        var onePointButton = new InlineKeyboardButton(ButtonTextConstants.DiceOnePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.MakeBetButtonCommand,
                CommandParam = 1
            })
        };
        var twoPointButton = new InlineKeyboardButton(ButtonTextConstants.DiceTwoPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.MakeBetButtonCommand,
                CommandParam = 2
            })
        };
        var threePointButton = new InlineKeyboardButton(ButtonTextConstants.DiceThreePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.MakeBetButtonCommand,
                CommandParam = 3
            })
        };
        var fourPointButton = new InlineKeyboardButton(ButtonTextConstants.DiceFourPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.MakeBetButtonCommand,
                CommandParam = 4
            })
        };
        var fivePointButton = new InlineKeyboardButton(ButtonTextConstants.DiceFivePointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.MakeBetButtonCommand,
                CommandParam = 5
            })
        };
        var sixPointButton = new InlineKeyboardButton(ButtonTextConstants.DiceSixPointButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.MakeBetButtonCommand,
                CommandParam = 6
            })
        };
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.GetGamesButtonCommand
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
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.DemoPlayFootballButtonCommand
            })
        };
        var playRealFootballButton = new InlineKeyboardButton(ButtonTextConstants.RealPlayFootBallButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.RealPlayFootballButtonCommand
            })
        };
        var backButton = new InlineKeyboardButton(ButtonTextConstants.BackButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandModel
            {
                CommandText = ButtonCommands.GetGamesButtonCommand
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