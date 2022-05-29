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
                Command = Command.Balance
            })
        };

        var buttonRows = new[]{ gamesButton, settingButton, balanceButton };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = GetBalanceMessage(chat);
    }

    public void InitGamesButtons()
    {
        var diceGameButton = new InlineKeyboardButton(_localizer[Resources.DiceGameButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseGame,
                Param = Command.PlayDice.ToString()
            })
        };
        var footballGameButton = new InlineKeyboardButton(_localizer[Resources.FootballGameButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseGame,
                Param = Command.PlayFootball.ToString()
            })
        };
        var basketballGameButton = new InlineKeyboardButton(_localizer[Resources.BasketballGameButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseGame,
                Param = Command.PlayBasketball.ToString()
            })
        };
        var dartsGameButton = new InlineKeyboardButton(_localizer[Resources.DartsGameButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseGame,
                Param = Command.PlayDarts.ToString()
            })
        };
        var bowlingGameButton = new InlineKeyboardButton(_localizer[Resources.BowlingGameButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChooseGame,
                Param = Command.PlayBowling.ToString()
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenu
            })
        };

        var buttonRows = new[] 
        {
            new[] { footballGameButton, diceGameButton },
            new[] { basketballGameButton, dartsGameButton },
            new []{ bowlingGameButton },
            new[] { backButton}
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.ChooseActionResource];
    }

    public void InitGetBalanceButtons(ChatModel chatModel)
    {
        var backButton = new InlineKeyboardButton( _localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenu
            })
        };
        var depositBalanceButton = new InlineKeyboardButton(_localizer[Resources.DepositBalance])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DepositMethods
            })
        };
        var withdrawBalanceButton = new InlineKeyboardButton(_localizer[Resources.WithdrawBalanceButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.WithdrawBalance
            })
        };

        var buttonRows = new[]
        {
            new [] {depositBalanceButton, withdrawBalanceButton},
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.GetMyBalanceResource, chatModel.Balance];
    }

    public void InitPlayGameButtons(GameModel gameModel, InitGameModel initGameModel)
    {
        var hitBallButton = new InlineKeyboardButton(initGameModel.PlayButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = initGameModel.PlayGameCommand,
                Param = JsonConvert.SerializeObject(new GameBetParamDto
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
                Command = Command.ChooseGame,
                Param = initGameModel.PlayToCommand.ToString()
            })
        };
        var betButtonsPanel = GetBetButtonsPanel(gameModel, initGameModel.ChangeGameBetCommand);

        var buttonRows = new[]
        {
            betButtonsPanel[0],
            betButtonsPanel[1],
            new[] { hitBallButton },
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = gameModel.IsDemoPlay
            ? _localizer[Resources.GetMyDemoBalanceResource, gameModel.Chat.DemoBalance]
            : _localizer[Resources.GetMyBalanceResource, gameModel.Chat.Balance];
    }

    public void InitDiceChooseBetButtons(GameModel gameModel, int chosenDice)
    {
        var onePointButton = new InlineKeyboardButton(GetDiceNumberButton(1, chosenDice, ButtonConstants.DiceOnePointButtonText))
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 1,   
                    Bet = gameModel.UserBet,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var twoPointButton = new InlineKeyboardButton(GetDiceNumberButton(2, chosenDice, ButtonConstants.DiceTwoPointButtonText))
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 2,
                    Bet = gameModel.UserBet,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var threePointButton = new InlineKeyboardButton(GetDiceNumberButton(3, chosenDice, ButtonConstants.DiceThreePointButtonText))
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 3,
                    Bet = gameModel.UserBet,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var fourPointButton = new InlineKeyboardButton(GetDiceNumberButton(4, chosenDice, ButtonConstants.DiceFourPointButtonText))
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 4,
                    Bet = gameModel.UserBet,
                    IsDemo =  Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var fivePointButton = new InlineKeyboardButton(GetDiceNumberButton(5, chosenDice, ButtonConstants.DiceFivePointButtonText))
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 5,
                    Bet = gameModel.UserBet,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var sixPointButton = new InlineKeyboardButton(GetDiceNumberButton(6, chosenDice, ButtonConstants.DiceSixPointButtonText))
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDiceBet,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = 6,
                    Bet = gameModel.UserBet,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var throwDiceButton = new InlineKeyboardButton(_localizer[Resources.ThrowDiceButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ThrowDice,
                Param = JsonConvert.SerializeObject(new DiceGameParamDto
                {
                    Dice = chosenDice,
                    Bet = gameModel.UserBet,
                    IsDemo = Convert.ToInt32(gameModel.IsDemoPlay)
                })
            })
        };
        var betButtonsPanel = GetBetButtonsPanel(gameModel, Command.ChangeDiceBet);

        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.GetGames
            })
        };

        var buttonRows = new[] 
        {
            betButtonsPanel[0],
            betButtonsPanel[1],
            new[] 
            { 
                onePointButton, twoPointButton, threePointButton, fourPointButton, fivePointButton, sixPointButton
            },
            new [] { throwDiceButton },
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = gameModel.IsDemoPlay
            ? _localizer[Resources.GetMyDemoBalanceResource, gameModel.Chat.DemoBalance]
            : _localizer[Resources.GetMyBalanceResource, gameModel.Chat.Balance];
    }
    public void InitChooseGameMode(Command commandToPlayGame)
    {
        var playDemoDiceButton = new InlineKeyboardButton(_localizer[Resources.DemoPlayButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = commandToPlayGame,
                Param = true.ToString()
            })
        };
        var playRealDiceButton = new InlineKeyboardButton(_localizer[Resources.RealPlayButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = commandToPlayGame,
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

        var switchToEnglishButton = new InlineKeyboardButton(AppConstants.BritainFlag)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.SwitchLanguage,
                Param = en
            })
        };
        var switchToRussianButton = new InlineKeyboardButton(AppConstants.RusFlag)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.SwitchLanguage,
                Param = ru
            })
        };
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenu
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
        ReplyText = currentLanguage == en
            ? _localizer[Resources.CurrentLanguageButtonText, currentLanguage, AppConstants.BritainFlag]
            : _localizer[Resources.CurrentLanguageButtonText, currentLanguage, AppConstants.RusFlag];
    }

    public void InitDepositByTonButtons()
    {
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DepositMethods
            })
        };
        var walletButton = new InlineKeyboardButton(_localizer[Resources.WalletButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.Balance
            }),
            Url = AppConstants.Wallet
        };
        var myWalletButton = new InlineKeyboardButton(_localizer[Resources.MyWalletButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.Balance
            }),
            Url = AppConstants.MyWalletUrl
        };
        var instructionButton = new InlineKeyboardButton(_localizer[Resources.DepositInstructionButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DepositInstruction
            }),
            Url = AppConstants.DepositInstruction
        };


        var buttonRows = new[]
        {
            new [] {walletButton, myWalletButton},
            new [] {instructionButton},
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.ShotStoryHowToDeposit];
    }

    public void InitWithdrawBalanceButtons(ChatModel chatModel, WithdrawModel withdrawModel)
    {
        InlineKeyboardButton[][] buttonRows;

        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.Balance
            })
        };

        if (IsBalanceEnoughToWithdraw(chatModel.Balance))
        {
            var percent25BalanceButton = GetChooseWithdrawAmountButton(chatModel, withdrawModel, 25);
            var percent50BalanceButton = GetChooseWithdrawAmountButton(chatModel, withdrawModel, 50);
            var percent75BalanceButton = GetChooseWithdrawAmountButton(chatModel, withdrawModel, 75);
            var wholeBalanceButton = GetChooseWithdrawAmountButton(chatModel, withdrawModel, 100);


            var tonMethodWithdraw = GetChooseWithdrawMethodButton(withdrawModel, Currency.TON, _localizer[Resources.TonWithdraw]);
            var cardMethodWithdraw = GetChooseWithdrawMethodButton(withdrawModel, Currency.USD, _localizer[Resources.CardButtonText]);

            var confirmWithdraw = new InlineKeyboardButton(_localizer[Resources.WithdrawBalanceButtonText])
            {
                CallbackData = JsonConvert.SerializeObject(new CommandDto
                {
                    Command = Command.ConfirmWithdraw,
                    Param = JsonConvert.SerializeObject(new WithdrawModel
                    {
                        Method = withdrawModel.Method,
                        Amount = withdrawModel.Amount
                    })
                })
            };

            buttonRows = new[]
            {
                new[] {percent25BalanceButton, percent50BalanceButton, percent75BalanceButton, wholeBalanceButton},
                new[] {tonMethodWithdraw, cardMethodWithdraw},
                new[] {confirmWithdraw},
                new[] {backButton}
            };
            ReplyText = string.Concat(
                _localizer[Resources.GetMyBalanceResource, chatModel.Balance],
                Environment.NewLine,
                _localizer[Resources.ChooseAmountToWithdraw, chatModel.Balance]);
        }
        else
        {
            buttonRows = new[]
            {
                new [] { backButton }
            };
            ReplyText = string.Concat(
                _localizer[Resources.DoNotHaveEnoughMoneyToWithdraw],
                Environment.NewLine,
                _localizer[Resources.GetMyBalanceResource, chatModel.Balance]);
        }

        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
    }

    public void SomethingWrongTryAgainButtons()
    {
        var backButton = new InlineKeyboardButton(_localizer[Resources.MenuButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenu
            })
        };
        
        var buttonRows = new[]
        {
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.SomethingWrongTryAgain];
    }

    public void InitWithdrawSuccessButtons()
    {
        var backButton = new InlineKeyboardButton(_localizer[Resources.MenuButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ToMenu
            })
        };

        var buttonRows = new[]
        {
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.WithdrawSuccess];
    }

    public void InitChooseDepositBalanceButtons(ChatModel chatModel)
    {
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.Balance
            })
        };
        var depositBalanceButton = new InlineKeyboardButton(_localizer[Resources.TonButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DepositByTon
            })
        };
        var withdrawBalanceButton = new InlineKeyboardButton(_localizer[Resources.CardButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DepositByCard
            })
        };

        var buttonRows = new[]
        {
            new [] {depositBalanceButton, withdrawBalanceButton},
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = string.Concat(_localizer[Resources.GetMyBalanceResource, chatModel.Balance],
            Environment.NewLine,
            _localizer[Resources.ChooseDepositMethod]);
    }

    public void InitChooseDepositByCardMethod(DepositModel depositModel, double balance)
    {
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DepositMethods
            })
        };
        var depositInvoiceButton = new InlineKeyboardButton(_localizer[Resources.DepositInvoice])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.CreateDepositInvoice,
                Param = JsonConvert.SerializeObject(depositModel)
            })
        };
        var depositPanelButton = new InlineKeyboardButton(_localizer[Resources.ChangeDepositParametersButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDepositAmount,
                Param = JsonConvert.SerializeObject(depositModel)
            })
        };

        var buttonRows = new[]
        {
            new [] { depositPanelButton},
            new [] { depositInvoiceButton },
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = string.Concat(_localizer[Resources.GetMyBalanceResource, balance],
            Environment.NewLine,
            _localizer[Resources.CurrentDepositParameter, depositModel.AmountCents, depositModel.Currency.ToString()]);
    }

    public void InitDepositPanel(DepositModel depositModel)
    {
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DepositByCard,
                Param = JsonConvert.SerializeObject(depositModel)
            })
        };

        var usdButton = new InlineKeyboardButton(GetCurrencyButtonText(DepositCurrency.USD, depositModel.Currency))
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDepositAmount,
                Param = JsonConvert.SerializeObject(new DepositDto
                {
                    AmountCents = AppConstants.UsdDefaultDepositIncreasingAmount,
                    Currency = DepositCurrency.USD
                })
            })
        };
        var euroButton = new InlineKeyboardButton(GetCurrencyButtonText(DepositCurrency.EUR, depositModel.Currency))
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDepositAmount,
                Param = JsonConvert.SerializeObject(new DepositDto
                {
                    AmountCents = AppConstants.EuroDefaultDepositIncreasingAmount,
                    Currency = DepositCurrency.EUR
                })
            })
        };
        var rubButton = new InlineKeyboardButton(GetCurrencyButtonText(DepositCurrency.RUB, depositModel.Currency))
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDepositAmount,
                Param = JsonConvert.SerializeObject(new DepositDto
                {
                    AmountCents = AppConstants.RubDefaultDepositIncreasingAmount,
                    Currency = DepositCurrency.RUB
                })
            })
        };
        var redoubleDepositButton = new InlineKeyboardButton(ButtonConstants.RedoubleAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDepositAmount,
                Param = JsonConvert.SerializeObject(new DepositDto
                {
                    AmountCents = depositModel.AmountCents * 2,
                    Currency = depositModel.Currency
                })
            })
        };
        var halvingDepositButton = new InlineKeyboardButton(ButtonConstants.HalvingAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDepositAmount,
                Param = JsonConvert.SerializeObject(new DepositDto
                {
                    AmountCents = depositModel.GetHalveAmount(),
                    Currency = depositModel.Currency
                })
            })
        };
        var increaseDepositButton = new InlineKeyboardButton(string.Concat(depositModel.GetDefaultChangeAmountString(),
            ButtonConstants.IncreaseAmountButtonText))
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDepositAmount,
                Param = JsonConvert.SerializeObject(new DepositDto
                {
                    AmountCents = depositModel.GetIncreasedAmount(),
                    Currency = depositModel.Currency
                })
            })
        };
        var decreaseDepositButton = new InlineKeyboardButton(string.Concat(depositModel.GetDefaultChangeAmountString(),
            ButtonConstants.DecreaseAmountButtonText))
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.ChangeDepositAmount,
                Param = JsonConvert.SerializeObject(new DepositDto
                {
                    AmountCents = depositModel.GetDecreaseAmount(),
                    Currency = depositModel.Currency
                })
            })
        };
        var currentUserDeposit = string.Concat(depositModel.ToString());
        var currentUserDepositButton = new InlineKeyboardButton(currentUserDeposit)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.DoNothing,
                Param = JsonConvert.SerializeObject(depositModel)
            })
        };

        var buttonRows = new[]
        {
            new [] { decreaseDepositButton, currentUserDepositButton, increaseDepositButton },
            new [] { halvingDepositButton, redoubleDepositButton },
            new [] { rubButton, usdButton, euroButton },
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = string.Concat(
            _localizer[Resources.ChooseDepositParameters],
            Environment.NewLine,
            _localizer[Resources.CurrentDepositParameter, depositModel.AmountCents, depositModel.Currency.ToString()]);
    }

    public void InitInvoiceButtons(DepositModel depositModel)
    {
        var backButton = new InlineKeyboardButton(_localizer[Resources.BackButtonText])
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.BackToDepositByCardFromDeposit,
                Param = JsonConvert.SerializeObject(depositModel)
            })
        };
        var depositButton = new InlineKeyboardButton(_localizer[Resources.DepositButtonText])
        {
            Pay = true
        };


        var buttonRows = new[]
        {
            new [] { depositButton},
            new [] { backButton }
        };
        GetInlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows);
        ReplyText = _localizer[Resources.CurrentDepositParameter, depositModel.AmountCents, depositModel.Currency.ToString()];
    }

    private string GetDiceNumberButton(int diceButtonFor, int chosenDice, string buttonText)
    {
        var diceIsChosen = diceButtonFor == chosenDice;

        return diceIsChosen 
            ? string.Concat("🔘", buttonText) 
            : buttonText;
    }

    private static string GetCurrencyButtonText(DepositCurrency forCurrency, DepositCurrency chosenCurrency)
    {
        var isCurrencyChosen = chosenCurrency == forCurrency;
        return isCurrencyChosen 
            ? string.Concat(ButtonConstants.IsChecked, " ", forCurrency.ToString())
            : forCurrency.ToString();
    }

    private InlineKeyboardButton[][] GetBetButtonsPanel(GameModel gameModel, Command changeDiceBet)
    {
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
                Command = changeDiceBet,
                Param = JsonConvert.SerializeObject(new GameBetParamDto
                {
                    Bet = GetAndCheckIfMoreThenMinimalUserBet(gameModel.UserBet + 1),
                    IsDemo = gameModel.IsDemoPlay
                })
            })
        };
        var decreaseBetButton = new InlineKeyboardButton(ButtonConstants.DecreaseAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = changeDiceBet,
                Param = JsonConvert.SerializeObject(new GameBetParamDto
                {
                    Bet = GetAndCheckIfMoreThenMinimalUserBet(gameModel.UserBet - 1),
                    IsDemo = gameModel.IsDemoPlay
                })
            })
        };
        var redoubleBetButton = new InlineKeyboardButton(ButtonConstants.RedoubleAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = changeDiceBet,
                Param = JsonConvert.SerializeObject(new GameBetParamDto
                {
                    Bet = gameModel.UserBet * 2,
                    IsDemo = gameModel.IsDemoPlay
                })
            })
        };
        var halvingBetButton = new InlineKeyboardButton(ButtonConstants.HalvingAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = changeDiceBet,
                Param = JsonConvert.SerializeObject(new GameBetParamDto
                {
                    Bet = GetAndCheckIfMoreThenMinimalUserBet(gameModel.UserBet / 2),
                    IsDemo = gameModel.IsDemoPlay
                })
            })
        };
        var maxBetButton = new InlineKeyboardButton(ButtonConstants.MaxAmountButtonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = changeDiceBet,
                Param = JsonConvert.SerializeObject(new GameBetParamDto
                {
                    Bet = gameModel.IsDemoPlay ? gameModel.Chat.DemoBalance : (int)gameModel.Chat.Balance,
                    IsDemo = gameModel.IsDemoPlay
                })
            })
        };
        var buttonsPanel = new[]
        {
            new[]
            {
                decreaseBetButton, currentUserBetButton, increaseBetButton
            },
            new[]
            {
                halvingBetButton, maxBetButton, redoubleBetButton
            }
        };

        return buttonsPanel;
    }

    private static int GetAndCheckIfMoreThenMinimalUserBet(int userBet)
    {
        return userBet >= AppConstants.MinBet 
            ? userBet
            : AppConstants.MinBet;
    }

    private InlineKeyboardButton GetChooseWithdrawMethodButton(WithdrawModel withdrawModel, Currency currency, string methodName)
    {
        var isClicked = IsWithdrawMethodButtonClick(withdrawModel, currency);
        var buttonText = isClicked
            ? string.Concat("✔️", methodName)
            : methodName;
        var tonMethodWithdraw = new InlineKeyboardButton(buttonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.WithdrawBalance,
                Param = JsonConvert.SerializeObject(new WithdrawModel
                {
                    Method = currency,
                    Amount = withdrawModel.Amount
                })
            })
        };
        return tonMethodWithdraw;
    }

    private InlineKeyboardButton GetChooseWithdrawAmountButton(ChatModel chatModel, WithdrawModel withdrawModel,
        int percentOfBalanceToWithdraw)
    {
        var buttonText = GetWithdrawAmountButtonText(chatModel, withdrawModel, percentOfBalanceToWithdraw);
        var percent25BalanceButton = new InlineKeyboardButton(buttonText)
        {
            CallbackData = JsonConvert.SerializeObject(new CommandDto
            {
                Command = Command.WithdrawBalance,
                Param = JsonConvert.SerializeObject(new WithdrawModel
                {
                    Method = withdrawModel.Method,
                    Amount = (int) (chatModel.Balance * percentOfBalanceToWithdraw / 100)
                })
            })
        };
        return percent25BalanceButton;
    }

    private string GetWithdrawAmountButtonText(ChatModel chatModel, WithdrawModel withdrawModel, int percentOfBalanceToWithdraw)
    {
        var amount = (int) (chatModel.Balance * percentOfBalanceToWithdraw / 100);
        var isClicked = IsWithdrawAmountButtonClicked(withdrawModel, amount);
        var buttonText = isClicked
            ? string.Concat("✔️", _localizer[Resources.AmountWithdraw, percentOfBalanceToWithdraw, amount])
            : _localizer[Resources.AmountWithdraw, percentOfBalanceToWithdraw, amount];
        return buttonText;
    }

    private bool IsWithdrawMethodButtonClick(WithdrawModel withdrawModel, Currency ton) => 
        withdrawModel.Method == ton;

    private static bool IsWithdrawAmountButtonClicked(WithdrawModel withdrawModel, int amount) => 
        withdrawModel.Amount == amount;

    private static bool IsBalanceEnoughToWithdraw(double balance) =>
        balance >= AppConstants.MinBalanceToWithdraw;
    private string GetBalanceMessage(ChatModel chat)
    {
        return string.Concat(
            _localizer[Resources.GetMyBalanceResource, chat.Balance],
            Environment.NewLine,
            _localizer[Resources.GetMyDemoBalanceResource, chat.DemoBalance],
            Environment.NewLine,
            _localizer[Resources.ChooseActionResource]);
    }
}