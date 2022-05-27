using System.Globalization;
using AutoMapper;
using Casino.BLL.ButtonsGenerators;
using Casino.BLL.ClickHandlers.Interfaces;
using Casino.BLL.Games;
using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;
using Casino.Common.AppConstants;
using Casino.Common.Dtos;
using Casino.Common.Enum;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Payments;

namespace Casino.BLL.ClickHandlers.Implementation;

public class ButtonClickHandler : IClickHandler
{
    private readonly TelegramMessageDto _telegramMessageDto;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly InlineKeyboardButtonsGenerator _inlineKeyboardButtonsGenerator;
    private readonly IChatsLanguagesInMemoryRepository _chatsLanguagesInMemoryRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IChatService _chatService;
    private readonly IWithdrawService _withdrawService;
    private readonly IMapper _mapper;
    private IStringLocalizer<Resources> _localizer;

    public ButtonClickHandler(TelegramMessageDto telegramMessageDto,
        ITelegramBotClient telegramBotClient, 
        IServiceProvider serviceProvider)
    {
        _telegramMessageDto = telegramMessageDto;
        _telegramBotClient = telegramBotClient;
        _serviceProvider = serviceProvider;
        _inlineKeyboardButtonsGenerator = serviceProvider.GetRequiredService<InlineKeyboardButtonsGenerator>();
        _chatsRepository = serviceProvider.GetRequiredService<IChatsRepository>();
        _chatsLanguagesInMemoryRepository = serviceProvider.GetRequiredService<IChatsLanguagesInMemoryRepository>();
        _chatService = serviceProvider.GetRequiredService<IChatService>();
        _withdrawService = serviceProvider.GetRequiredService<IWithdrawService>();
        _mapper = serviceProvider.GetRequiredService<IMapper>();
        _localizer = serviceProvider.GetRequiredService<IStringLocalizer<Resources>>();
    }

    public async Task PushButtonAsync()
    {
        var commandDto = _telegramMessageDto.CommandDto;
        switch (commandDto.Command)
        {
            case Command.GetGames:
                await PushGamesButtonAsync();
                break;
            case Command.Settings:
                await PushSettingsButtonAsync();
                break;
            case Command.Start:
                await StartBotAsync();
                break;
            case Command.StartAfterDepositing:
                await PushStartAfterDepositingButtonAsync();
                break;
            case Command.ToMenu:
                await PushMenuButtonAsync();
                break;
            case Command.Balance:
                await PushGetBalanceButtonAsync();
                break;
            case Command.ChooseGame:
                var playToCommand = Enum.Parse<Command>(commandDto.Param!);
                await PushChooseGameAsync(playToCommand);
                break;
            case Command.ThrowDarts:
                var dartsGameParam = JsonConvert.DeserializeObject<GameBetParamDto>(commandDto.Param);
                await PushThrowDartButtonAsync(dartsGameParam);
                break;
            case Command.PlayDice:
                var isDemoDicePlay = bool.Parse(commandDto.Param!);
                await PushDiceButtonAsync(isDemoDicePlay);
                break;
            case Command.PlayDarts:
                var isDemoDartsPlay = bool.Parse(commandDto.Param!);
                await PushDartsButtonAsync(isDemoDartsPlay);
                break;
            case Command.PlayBasketball:
                var isDemoBasketPlay = bool.Parse(commandDto.Param!);
                await PushBasketButtonAsync(isDemoBasketPlay);
                break;
            case Command.PlayFootball:
                var isDemoFootballPlay = bool.Parse(commandDto.Param!);
                await PushFootballPlayButtonAsync(isDemoFootballPlay);
                break;
            case Command.PlayBowling:
                var isDemoBowlingPlay = bool.Parse(commandDto.Param!);
                await PushBowlingPlayButtonAsync(isDemoBowlingPlay);
                break;
            case Command.HitBall:
                var footballGameParam = JsonConvert.DeserializeObject<GameBetParamDto>(commandDto.Param);
                await PushHitBallButtonAsync(footballGameParam);
                break;
            case Command.ThrowBasketBall:
                var basketGameParam = JsonConvert.DeserializeObject<GameBetParamDto>(commandDto.Param);
                await PushThrowBasketballButtonAsync(basketGameParam);
                break;
            case Command.DiceBet:
                var diceGameParamDto = JsonConvert.DeserializeObject<DiceGameParamDto>(commandDto.Param);
                await PushChooseDiceAsync(diceGameParamDto);
                break;
            case Command.HitBowlingBall:
                var bowlingGameParam = JsonConvert.DeserializeObject<GameBetParamDto>(commandDto.Param);
                await PushHitBowlingBallAsync(bowlingGameParam);
                break;
            case Command.ChangeDiceBet:
                var diceGameParam = JsonConvert.DeserializeObject<GameBetParamDto>(commandDto.Param!);
                await PushDiceButtonAsync(Convert.ToBoolean(diceGameParam.IsDemo), diceGameParam.Bet);
                break;
            case Command.ChangeFootballBet:
                var footballGame = JsonConvert.DeserializeObject<GameBetParamDto>(commandDto.Param!);
                await PushFootballPlayButtonAsync(footballGame.IsDemo, footballGame.Bet);
                break;
            case Command.ChangeDartsBet:
                var dartsGame = JsonConvert.DeserializeObject<GameBetParamDto>(commandDto.Param!);
                await PushDartsButtonAsync(dartsGame.IsDemo, dartsGame.Bet);
                break;
            case Command.ChangeBasketBet:
                var basketGame = JsonConvert.DeserializeObject<GameBetParamDto>(commandDto.Param!);
                await PushBasketButtonAsync(basketGame.IsDemo, basketGame.Bet);
                break;
            case Command.ChangeBowlingBet:
                var bowlingGame = JsonConvert.DeserializeObject<GameBetParamDto>(commandDto.Param!);
                await PushBowlingPlayButtonAsync(bowlingGame.IsDemo, bowlingGame.Bet);
                break;
            case Command.DepositByTon:
                await PushDepositByTonButtonAsync();  
                break;
            case Command.WithdrawBalance:
                var withdrawModel =  commandDto.Param == null ? null : JsonConvert.DeserializeObject<WithdrawModel>(commandDto.Param!);
                await PushWithdrawBalanceButtonAsync(withdrawModel);
                break;
            case Command.SwitchLanguage:
                var language = commandDto.Param;
                await PushSwitchLanguageAsync(language!);
                break;
            case Command.ConfirmWithdraw:
                var withdraw = JsonConvert.DeserializeObject<WithdrawModel>(commandDto.Param!);
                await PushConfirmWithdrawAsync(withdraw);
                break;
            case Command.DepositMethods:
                await PushChooseDepositMethodAsync();
                break;
            case Command.DepositByCard:
                var deposit = commandDto.Param != null ? JsonConvert.DeserializeObject<DepositDto>(commandDto.Param!) : null;
                await PushDepositByCardButtonAsync(deposit);
                break;
            case Command.ChangeDepositAmount:
                var depositDto = JsonConvert.DeserializeObject<DepositDto>(commandDto.Param!);
                await PushChangeDepositParametersAsync(depositDto);
                break;
            case Command.CreateDepositInvoice:
                var depositAmount = JsonConvert.DeserializeObject<DepositDto>(commandDto.Param!);
                await PushCreateDepositInvoiceAsync(depositAmount);
                break;
            case Command.BackToDepositByCardFromDeposit:
                await PushBackFromDepositButtonAsync();
                break;
            case Command.DoNothing:
                await _telegramBotClient.AnswerCallbackQueryAsync(_telegramMessageDto.CallbackQueryId);
                break;
        }
    }

    private async Task PushHitBowlingBallAsync(GameBetParamDto bowlingGameParam)
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            GameId = (int)Common.Enum.Games.Bowling,
            Chat = chatModel,
            UserBet = bowlingGameParam.Bet,
            IsDemoPlay = bowlingGameParam.IsDemo
        };
        var footBallGame = new BowlingGame(gameModel, _telegramMessageDto.MessageId,
            _telegramBotClient, _serviceProvider);
        await footBallGame.PlayRoundAsync();
    }

    private async Task PushBowlingPlayButtonAsync(bool isDemoBowlingPlay, int userBet = AppConstants.MinBet)
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            Chat = chatModel,
            UserBet = userBet,
            IsDemoPlay = isDemoBowlingPlay
        };
        var initGameModel = new InitGameModel
        {
            ChangeGameBetCommand = Command.ChangeBowlingBet,
            PlayGameCommand = Command.HitBowlingBall,
            PlayToCommand = Command.PlayBowling,
            PlayButtonText = _localizer[Resources.HitBowlingBallButtonText]
        };
        _inlineKeyboardButtonsGenerator.InitPlayGameButtons(gameModel, initGameModel);
        await EditCurrentScreenAsync();
    }

    private async Task PushThrowBasketballButtonAsync(GameBetParamDto basketGameParam)
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            GameId = (int)Common.Enum.Games.Basketball,
            Chat = chatModel,
            UserBet = basketGameParam.Bet,
            IsDemoPlay = basketGameParam.IsDemo
        };
        var footBallGame = new BasketBallGame(gameModel, _telegramMessageDto.MessageId,
            _telegramBotClient, _serviceProvider);
        await footBallGame.PlayRoundAsync();
    }

    private async Task PushBasketButtonAsync(bool isDemoBasketPlay, int userDartsBet = AppConstants.MinBet)
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            Chat = chatModel,
            UserBet = userDartsBet,
            IsDemoPlay = isDemoBasketPlay
        };
        var initGameModel = new InitGameModel
        {
            ChangeGameBetCommand = Command.ChangeBasketBet,
            PlayGameCommand = Command.ThrowBasketBall,
            PlayToCommand = Command.PlayBasketball,
            PlayButtonText = _localizer[Resources.ThrowBasketBallButtonText]
        };
        _inlineKeyboardButtonsGenerator.InitPlayGameButtons(gameModel, initGameModel);
        await EditCurrentScreenAsync();
    }

    private async Task PushStartAfterDepositingButtonAsync()
    {
        await _telegramBotClient.DeleteMessageAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId - 1); 
        await StartBotAsync();
    }

    private async Task PushBackFromDepositButtonAsync()
    {
        var depositModel = new DepositModel
        {
            AmountCents = AppConstants.DefaultDepositAmount,
            Currency = AppConstants.DefaultDepositCurrency
        };
        var balance = (await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId)).Balance;
        _inlineKeyboardButtonsGenerator.InitChooseDepositByCardMethod(depositModel, balance);
        await _telegramBotClient.DeleteMessageAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId);
        await _telegramBotClient.SendTextMessageAsync(_telegramMessageDto.ChatId, _inlineKeyboardButtonsGenerator.ReplyText,
            replyMarkup: _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup);
    }

    private async Task PushCreateDepositInvoiceAsync(DepositDto depositAmount)
    {
        var depositModel = _mapper.Map<DepositModel>(depositAmount);
        _inlineKeyboardButtonsGenerator.InitInvoiceButtons(depositModel);

        await _telegramBotClient.DeleteMessageAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId);
        await _telegramBotClient.SendInvoiceAsync(_telegramMessageDto.ChatId,
            "title", "desc", _telegramMessageDto.ChatId.ToString(),
            AppConstants.UnlimintTestTokenProvider,
            depositAmount.Currency.ToString(),
            new[] {new LabeledPrice(depositAmount.Currency.ToString(), depositAmount.AmountCents * 100)},
            replyMarkup: _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup);
    }

    private async Task PushDepositByCardButtonAsync(DepositDto? depositDto)
    {
        var depositModel = depositDto == null
            ? new DepositModel
            {
                AmountCents = AppConstants.DefaultDepositAmount,
                Currency = AppConstants.DefaultDepositCurrency
            }
            : _mapper.Map<DepositModel>(depositDto);

        var balance = (await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId)).Balance;
        _inlineKeyboardButtonsGenerator.InitChooseDepositByCardMethod(depositModel, balance);
        await EditCurrentScreenAsync();
    }

    private async Task PushChangeDepositParametersAsync(DepositDto depositDto)
    {
        var depositModel = _mapper.Map<DepositModel>(depositDto);
        _inlineKeyboardButtonsGenerator.InitDepositPanel(depositModel);
        await EditCurrentScreenAsync();
    }

    private async Task PushChooseDepositMethodAsync()
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        _inlineKeyboardButtonsGenerator.InitChooseDepositBalanceButtons(chatModel);
        await EditCurrentScreenAsync();
    }

    private async Task PushThrowDartButtonAsync(GameBetParamDto dartsGameParam)
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            GameId = (int)Common.Enum.Games.Darts,
            Chat = chatModel,
            UserBet = dartsGameParam.Bet,
            IsDemoPlay = dartsGameParam.IsDemo
        };
        var footBallGame = new DartsGame(gameModel, _telegramMessageDto.MessageId,
            _telegramBotClient, _serviceProvider);
        await footBallGame.PlayRoundAsync();
    }

    private async Task PushDartsButtonAsync(bool isDemoDartsPlay, int userDartsBet = AppConstants.MinBet)
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            Chat = chatModel,
            UserBet = userDartsBet,
            IsDemoPlay = isDemoDartsPlay
        };
        var initGameModel = new InitGameModel
        {
            PlayButtonText = _localizer[Resources.ThrowDartButtonText],
            ChangeGameBetCommand = Command.ChangeDartsBet,
            PlayToCommand = Command.PlayDarts,
            PlayGameCommand = Command.ThrowDarts
        };
        _inlineKeyboardButtonsGenerator.InitPlayGameButtons(gameModel, initGameModel);
        await EditCurrentScreenAsync();
    }

    private async Task PushConfirmWithdrawAsync(WithdrawModel withdrawModel)
    {
        var withdrawResult =  await _withdrawService.WithdrawAsync(withdrawModel, _telegramMessageDto.ChatId);
        if (withdrawResult.IsSuccess)
        {
            _inlineKeyboardButtonsGenerator.InitWithdrawSuccessButtons();
        }
        else
        {
            _inlineKeyboardButtonsGenerator.SomethingWrongTryAgainButtons();
        }
        await EditCurrentScreenAsync();
    }

    private async Task PushWithdrawBalanceButtonAsync(WithdrawModel? withdrawModel = null)
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        withdrawModel ??= new()
        {
            Amount = (int)(chatModel.Balance * AppConstants.MinPercentOfBalanceToWithdraw / 100),
            Method = Currency.TON
        };
        
        _inlineKeyboardButtonsGenerator.InitWithdrawBalanceButtons(chatModel, withdrawModel);
        await EditCurrentScreenAsync();
    }

    private async Task PushDepositByTonButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitDepositByTonButtons();
        await EditCurrentScreenAsync();
    }

    private async Task PushSwitchLanguageAsync(string newLanguage)
    {
        if (CultureInfo.CurrentUICulture.ToString() == newLanguage)
            return;

        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(newLanguage);
        _chatsLanguagesInMemoryRepository.AddOrUpdateChatLanguage(_telegramMessageDto.ChatId, newLanguage);
        await _chatsRepository.UpdateChatLanguageAsync(_telegramMessageDto.ChatId, newLanguage);
        await PushSettingsButtonAsync();
    }

    private async Task PushSettingsButtonAsync()
    {
        var currentLanguage = _chatsLanguagesInMemoryRepository.GetChatLanguage(_telegramMessageDto.ChatId);
        _inlineKeyboardButtonsGenerator.InitSettingsButtons(currentLanguage);
        await EditCurrentScreenAsync();
    }

    private async Task PushChooseGameAsync(Command playToCommand)
    {
        _inlineKeyboardButtonsGenerator.InitChooseGameMode(playToCommand);
        await EditCurrentScreenAsync();
    }

    private async Task PushChooseDiceAsync(DiceGameParamDto diceGameParam)
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            GameId = (int)Common.Enum.Games.Dice,
            Chat = chatModel,
            UserBet = diceGameParam.Bet,
            IsDemoPlay = Convert.ToBoolean(diceGameParam.IsDemo)
        };
        var diceGame = new DiceGame(gameModel, _telegramMessageDto.MessageId, _telegramBotClient
            ,diceGameParam.Dice, _serviceProvider);
        await diceGame.PlayRoundAsync();
    }

    private async Task StartBotAsync()
    {
        var chatModel = await _chatService.GetOrCreateChatIfNotExistAsync(_telegramMessageDto.ChatId);
        _inlineKeyboardButtonsGenerator.InitStartButtons(chatModel);
        await _telegramBotClient.SendTextMessageAsync(_telegramMessageDto.ChatId, _inlineKeyboardButtonsGenerator.ReplyText,
            replyMarkup: _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup);
    }

    private async Task PushDiceButtonAsync(bool isDemoPlay, int userDiceBet = AppConstants.MinBet)
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            Chat = chatModel,
            UserBet = userDiceBet,
            IsDemoPlay = isDemoPlay
        };
        _inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons(gameModel);
        await EditCurrentScreenAsync();
    }

    private async Task PushHitBallButtonAsync(GameBetParamDto gameBetParam)
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            GameId = (int) Common.Enum.Games.Football,
            Chat = chatModel,
            UserBet = gameBetParam.Bet,
            IsDemoPlay = gameBetParam.IsDemo
        };
        var footBallGame = new FootBallGame(gameModel, _telegramMessageDto.MessageId,
            _telegramBotClient, _serviceProvider);
        await footBallGame.PlayRoundAsync();
    }

    private async Task PushFootballPlayButtonAsync(bool isDemoPlay, int userBet = AppConstants.MinBet)
    {
        var chatModel = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            Chat =  chatModel,
            UserBet = userBet,
            IsDemoPlay = isDemoPlay
        };
        var initGameModel = new InitGameModel
        {
            ChangeGameBetCommand = Command.ChangeFootballBet,
            PlayGameCommand = Command.HitBall,
            PlayToCommand = Command.PlayFootball,
            PlayButtonText = _localizer[Resources.HitFootBallButtonText]
        };
        _inlineKeyboardButtonsGenerator.InitPlayGameButtons(gameModel, initGameModel);
        await EditCurrentScreenAsync();
    }

    private async Task PushGetBalanceButtonAsync()
    {
        var chat = await _chatsRepository.GetChatByIdAsync(_telegramMessageDto.ChatId);
        var chatModel = new ChatModel
        {
            Balance = chat.Balance,
            DemoBalance = chat.DemoBalance,
            Id = chat.Id
        };
        _inlineKeyboardButtonsGenerator.InitGetBalanceButtons(chatModel);
        await EditCurrentScreenAsync();
    }

    private async Task PushMenuButtonAsync()
    {
        var chat = await _chatsRepository.GetChatByIdAsync(_telegramMessageDto.ChatId);
        var chatModel = new ChatModel
        {
            Balance = chat.Balance,
            DemoBalance = chat.DemoBalance,
            Id = chat.Id
        };
        _inlineKeyboardButtonsGenerator.InitStartButtons(chatModel);
        await EditCurrentScreenAsync();
    }

    private async Task PushGamesButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitGamesButtons();
        await EditCurrentScreenAsync();
    }

    private async Task EditCurrentScreenAsync()
    {
        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId,
            _inlineKeyboardButtonsGenerator.ReplyText,
            replyMarkup: _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup);
    }
}