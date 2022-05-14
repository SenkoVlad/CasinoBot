using System.Globalization;
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
using Newtonsoft.Json;
using Telegram.Bot;

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
            case Command.ToMenuButton:
                await PushMenuButtonAsync();
                break;
            case Command.ToBalanceButton:
            case Command.GetBalance:
                await PushGetBalanceButtonAsync();
                break;
            case Command.ChooseFootball:
                await PushChooseFootballGame();
                break;
            case Command.ChooseDice:
                await PushChooseDiceGame();
                break;
            case Command.ChooseDarts:
                await PushChooseDartsGame();
                break;
            case Command.HitBall:
                var footballGameParam = JsonConvert.DeserializeObject<GameBetParamDto>(commandDto.Param);
                await PushHitBallButtonAsync(footballGameParam);
                break;
            case Command.ThrowDart:
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
            case Command.PlayFootball:
                var isDemoFootballPlay = bool.Parse(commandDto.Param!);
                await PushFootballPlayButtonAsync(isDemoFootballPlay);
                break;
            case Command.DiceBet:
                var diceGameParamDto = JsonConvert.DeserializeObject<DiceGameParamDto>(commandDto.Param);
                await PushChooseDiceAsync(diceGameParamDto);
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
            case Command.DepositBalance:
                await PushDepositBalanceButtonAsync();  
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
            case Command.DoNothing:
                break;
        }
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
        _inlineKeyboardButtonsGenerator.InitDartsChooseBetButtons(gameModel);
        await EditCurrentScreenAsync();
    }

    private async Task PushChooseDartsGame()
    {
        _inlineKeyboardButtonsGenerator.InitChooseDartsMode();
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

    private async Task PushDepositBalanceButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitDepositBalanceButtons();
        await EditCurrentScreenAsync();
    }

    private async Task PushChooseDiceGame()
    {
        _inlineKeyboardButtonsGenerator.InitChooseDiceMode();
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

    private async Task PushChooseFootballGame()
    {
        _inlineKeyboardButtonsGenerator.InitChooseFootballMode();
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
        _inlineKeyboardButtonsGenerator.InitPlayFootballButtons(gameModel);
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