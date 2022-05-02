using System.Globalization;
using Casino.BLL.ButtonsGenerators;
using Casino.BLL.ClickHandlers.Interfaces;
using Casino.BLL.Games;
using Casino.BLL.Models;
using Casino.BLL.Services.Implementation;
using Casino.BLL.Services.Interfaces;
using Casino.Common.AppConstants;
using Casino.Common.Dtos;
using Casino.Common.Enum;
using Casino.Configuration.Configuration;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ClickHandlers.Implementation;

public class ButtonClickHandler : IClickHandler
{
    private readonly TelegramMessageDto _telegramMessageDto;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly InlineKeyboardButtonsGenerator _inlineKeyboardButtonsGenerator;
    private InlineKeyboardMarkup? _inlineKeyboardButtons;
    private string? _replyText;
    private readonly IStringLocalizer<Resources> _localizer;
    private readonly IChatsLanguagesInMemoryRepository _chatsLanguagesInMemoryRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IChatService _chatService;
    private readonly GameParameters _gameParametersService;
    public string? GetReplyText => _replyText;
    public ReplyKeyboardMarkup GetReplyKeyboardButtons => null!;

    public ButtonClickHandler(TelegramMessageDto telegramMessageDto,
        ITelegramBotClient telegramBotClient, 
        IServiceProvider serviceProvider)
    {
        _telegramMessageDto = telegramMessageDto;
        _telegramBotClient = telegramBotClient;
        _serviceProvider = serviceProvider;
        _inlineKeyboardButtonsGenerator = serviceProvider.GetRequiredService<InlineKeyboardButtonsGenerator>();
        _localizer = serviceProvider.GetRequiredService<IStringLocalizer<Resources>>();
        _chatRepository = serviceProvider.GetRequiredService<IChatRepository>();
        _chatsLanguagesInMemoryRepository = serviceProvider.GetRequiredService<IChatsLanguagesInMemoryRepository>();
        _chatService = serviceProvider.GetRequiredService<IChatService>();
        _gameParametersService = serviceProvider.GetRequiredService<GameParameters>();
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
            case Command.GetBalance:
                await PushGetBalanceButtonAsync();
                break;
            case Command.ChooseFootball:
                await PushChooseFootballGame();
                break;
            case Command.ChooseDice:
                await PushChooseDiceGame();
                break;
            case Command.HitBall:
                var footballGameParam = JsonConvert.DeserializeObject<FootballGameParamDto>(commandDto.Param);
                await PushHitBallButtonAsync(footballGameParam);
                break;
            case Command.PlayDice:
                var isDemoDicePlay = bool.Parse(commandDto.Param!);
                await PushDiceButtonAsync(isDemoDicePlay);
                break;
            case Command.PlayFootball:
                var isDemoFootballPlay = bool.Parse(commandDto.Param!);
                await PushFootballPlayButtonAsync(isDemoFootballPlay);
                break;
            case Command.DiceBet:
                var diceGameParamDto = JsonConvert.DeserializeObject<DiceGameParamDto>(commandDto.Param);
                await PushChooseDiceAsync(diceGameParamDto);
                break;
            case Command.IncreaseBalance:
            case Command.DecreaseBalance:
                var newPayment = int.Parse(commandDto.Param!);
                await PushGetBalanceButtonAsync(newPayment);
                break;
            case Command.IncreaseDiceBet:
            case Command.DecreaseDiceBet:
                var diceGameParam = JsonConvert.DeserializeObject<DiceGameParamDto>(commandDto.Param!);
                await PushDiceButtonAsync(Convert.ToBoolean(diceGameParam.IsDemo), diceGameParam.Bet);
                break;
            case Command.IncreaseFootballBet:
            case Command.DecreaseFootballBet:
                var footballGame = JsonConvert.DeserializeObject<FootballGameParamDto>(commandDto.Param!);
                await PushFootballPlayButtonAsync(footballGame.IsDemo, footballGame.Bet);
                break;
            case Command.DepositPayment:
                var deposit = int.Parse(commandDto.Param!);
                await PushDepositButtonAsync(deposit);
                break;
            case Command.SwitchLanguage:
                var language = commandDto.Param;
                await PushSwitchLanguageAsync(language!);
                break;
            case Command.DoNothing:
                break;
        }
    }

    private async Task PushChooseDiceGame()
    {
        _inlineKeyboardButtonsGenerator.InitChooseDiceMode();
        _replyText = _inlineKeyboardButtonsGenerator.ReplyText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushSwitchLanguageAsync(string newLanguage)
    {
        if (CultureInfo.CurrentUICulture.ToString() == newLanguage)
            return;

        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(newLanguage);
        _chatsLanguagesInMemoryRepository.AddOrUpdateChatLanguage(_telegramMessageDto.ChatId, newLanguage);
        await _chatRepository.UpdateChatLanguageAsync(_telegramMessageDto.ChatId, newLanguage);
        await PushSettingsButtonAsync();
    }

    private async Task PushSettingsButtonAsync()
    {
        var currentLanguage = _chatsLanguagesInMemoryRepository.GetChatLanguage(_telegramMessageDto.ChatId);
        _inlineKeyboardButtonsGenerator.InitSettingsButtons(currentLanguage);
        _replyText = _inlineKeyboardButtonsGenerator.ReplyText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushDepositButtonAsync(int deposit)
    { 
       await _chatRepository.ChangeBalanceAsync(_telegramMessageDto.ChatId, deposit);
       await PushGetBalanceButtonAsync();
    }

    private async Task PushChooseFootballGame()
    {
        _inlineKeyboardButtonsGenerator.InitChooseFootballMode();
        _replyText = _inlineKeyboardButtonsGenerator.ReplyText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushChooseDiceAsync(DiceGameParamDto diceGameParam)
    {
        var chat = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            GameId = (int)Common.Enum.Games.Dice,
            Chat = new ChatModel
            {
                Id = _telegramMessageDto.ChatId,
                Balance = chat.Balance,
                DemoBalance = chat.DemoBalance
            },
            UserBet = diceGameParam.Bet,
            IsDemoPlay = Convert.ToBoolean(diceGameParam.IsDemo)
        };
        var diceGame = new DiceGame(gameModel, _telegramMessageDto.MessageId, _telegramBotClient, 
            _chatService ,diceGameParam.Dice, _serviceProvider);
        await diceGame.PlayRoundAsync();
    }

    private async Task StartBotAsync()
    {
        var chat = await _chatService.GetOrCreateChatIfNotExistAsync(_telegramMessageDto.ChatId);
        var chatModel = new ChatModel
        {
            Balance = chat.Balance,
            DemoBalance = chat.DemoBalance,
            Id = chat.Id
        };
        _inlineKeyboardButtonsGenerator.InitStartButtons(chatModel);
        _replyText = _inlineKeyboardButtonsGenerator.ReplyText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.SendTextMessageAsync(_telegramMessageDto.ChatId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushDiceButtonAsync(bool isDemoPlay, int userDiceBet = AppConstants.DefaultUserBet)
    {
        var chat = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            Chat = new ChatModel
            {
                Id = _telegramMessageDto.ChatId,
                Balance = chat.Balance,
                DemoBalance = chat.DemoBalance
            },
            UserBet = userDiceBet,
            IsDemoPlay = isDemoPlay
        };
        _inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons(gameModel);
        _replyText = _inlineKeyboardButtonsGenerator.ReplyText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    } 

    private async Task PushHitBallButtonAsync(FootballGameParamDto footballGameParam)
    {
        var chat = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            GameId = (int) Common.Enum.Games.Football,
            Chat = new ChatModel
            {
                Id = _telegramMessageDto.ChatId,
                Balance = chat!.Balance,
                DemoBalance = chat.DemoBalance
            },
            UserBet = footballGameParam.Bet,
            IsDemoPlay = footballGameParam.IsDemo
        };
        var footBallGame = new FootBallGame(gameModel, _telegramMessageDto.MessageId,
            _telegramBotClient, _chatService, _serviceProvider);
        await footBallGame.PlayRoundAsync();
    }

    private async Task PushFootballPlayButtonAsync(bool isDemoPlay, int userBet = AppConstants.DefaultUserBet)
    {
        var chat = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            Chat = new ChatModel
            {
                Id = chat!.Id,
                Balance = chat.Balance,
                DemoBalance = chat.DemoBalance
            },
            UserBet = userBet,
            IsDemoPlay = isDemoPlay
        };
        _inlineKeyboardButtonsGenerator.InitPlayFootballButtons(gameModel);
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = _inlineKeyboardButtonsGenerator.ReplyText; 
        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId,
            _telegramMessageDto.MessageId, 
            text: _replyText,replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushGetBalanceButtonAsync(int payment = AppConstants.DefaultBalancePayment)
    {
        var chat = await _chatRepository.GetChatByIdAsync(_telegramMessageDto.ChatId);
        var chatModel = new ChatModel
        {
            Balance = chat.Balance,
            DemoBalance = chat.DemoBalance,
            Id = chat.Id
        };
        _inlineKeyboardButtonsGenerator.InitGetBalanceButtons(payment, chatModel);
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = _inlineKeyboardButtonsGenerator.ReplyText;

        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId, 
            _replyText, replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushMenuButtonAsync()
    {
        var chat = await _chatRepository.GetChatByIdAsync(_telegramMessageDto.ChatId);
        var chatModel = new ChatModel
        {
            Balance = chat.Balance,
            DemoBalance = chat.DemoBalance,
            Id = chat.Id
        };
        _inlineKeyboardButtonsGenerator.InitStartButtons(chatModel);
        _replyText = _inlineKeyboardButtonsGenerator.ReplyText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushGamesButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitGamesButtons();
        _replyText = _inlineKeyboardButtonsGenerator.ReplyText;
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId,
            _localizer[Resources.ChooseActionMessageText], replyMarkup: _inlineKeyboardButtons);
    }
}