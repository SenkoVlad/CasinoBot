using System.Globalization;
using Casino.BLL.ButtonsGenerators;
using Casino.BLL.ClickHandlers.Interfaces;
using Casino.BLL.Games;
using Casino.BLL.Models;
using Casino.BLL.Services.Interfaces;
using Casino.Common.AppConstants;
using Casino.Common.Dtos;
using Casino.Common.Enum;
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
    private readonly IBalanceRepository _balanceRepository;
    private readonly InlineKeyboardButtonsGenerator _inlineKeyboardButtonsGenerator;
    private InlineKeyboardMarkup? _inlineKeyboardButtons;
    private string? _replyText;
    private readonly IStringLocalizer<Resources> _localizer;
    private readonly IChatsLanguagesInMemoryRepository _chatsLanguagesInMemoryRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IChatService _chatService;
    public string? GetReplyText => _replyText;
    public ReplyKeyboardMarkup GetReplyKeyboardButtons => null!;

    public ButtonClickHandler(TelegramMessageDto telegramMessageDto,
        ITelegramBotClient telegramBotClient, 
        IServiceProvider serviceProvider)
    {
        _telegramMessageDto = telegramMessageDto;
        _telegramBotClient = telegramBotClient;
        _serviceProvider = serviceProvider;
        _balanceRepository = serviceProvider.GetRequiredService<IBalanceRepository>();
        _inlineKeyboardButtonsGenerator = serviceProvider.GetRequiredService<InlineKeyboardButtonsGenerator>();
        _localizer = serviceProvider.GetRequiredService<IStringLocalizer<Resources>>();
        _chatRepository = serviceProvider.GetRequiredService<IChatRepository>();
        _chatsLanguagesInMemoryRepository = serviceProvider.GetRequiredService<IChatsLanguagesInMemoryRepository>();
        _chatService = serviceProvider.GetRequiredService<IChatService>();
    }

    public async Task PushButtonAsync()
    {
        var commandDto = _telegramMessageDto.CommandDto;
        switch (commandDto.Command)
        {
            case Command.GetGamesButtonCommand:
                await PushGamesButtonAsync();
                break;
            case Command.SettingsCommand:
                await PushSettingsButtonAsync();
                break;
            case Command.StartCommand:
                await StartBotAsync();
                break;
            case Command.ToMenuButtonCommand:
                await PushMenuButtonAsync();
                break;
            case Command.GetBalanceButtonCommand:
                await PushGetBalanceButtonAsync();
                break;
            case Command.ChooseFootballGameButtonCommand:
                await PushChooseFootballGame();
                break;
            case Command.HitBallButtonCommand:
                var footballGameParam = JsonConvert.DeserializeObject<FootballGameParamDto>(commandDto.CommandParam);
                await PushHitBallButtonAsync(footballGameParam);
                break;
            case Command.ChooseDiceGameButtonCommand:
                await PushDiceButtonAsync(isDemoPlay:true);
                break;
            case Command.PlatFootballButtonCommand:
                var isDemoPlay = bool.Parse(commandDto.CommandParam!);
                await PushFootballPlayButtonAsync(isDemoPlay);
                break;
            case Command.MakeBetButtonCommand:
                var diceGameParamDto = JsonConvert.DeserializeObject<DiceGameParamDto>(commandDto.CommandParam);
                await PushChooseDice(diceGameParamDto.Dice, diceGameParamDto.UserBet);
                break;
            case Command.IncreaseBalancePayment:
            case Command.DecreaseBalancePayment:
                var newPayment = int.Parse(commandDto.CommandParam!);
                await PushGetBalanceButtonAsync(newPayment);
                break;
            case Command.IncreaseDiceBetPayment:
            case Command.DecreaseDiceBetPayment:
                var diceGameParam = JsonConvert.DeserializeObject<DiceGameParamDto>(commandDto.CommandParam!);
                await PushDiceButtonAsync(diceGameParam.IsDemoPlay, diceGameParam.UserBet);
                break;
            case Command.IncreaseFootballBetPayment:
            case Command.DecreaseFootballBetPayment:
                var footballGame = JsonConvert.DeserializeObject<FootballGameParamDto>(commandDto.CommandParam!);
                await PushDiceButtonAsync(footballGame.IsDemoPlay, footballGame.UserBet);
                break;
            case Command.DepositPayment:
                var deposit = int.Parse(commandDto.CommandParam!);
                await PushDepositButtonAsync(deposit);
                break;
            case Command.SwitchLanguageCommand:
                var language = commandDto.CommandParam;
                await PushSwitchLanguageAsync(language!);
                break;
            case Command.DoNothing:
                await _telegramBotClient.AnswerCallbackQueryAsync("Empty");
                break;
        }
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
       _balanceRepository.AddScoreToBalanceAsync(_telegramMessageDto.ChatId, deposit);
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

    private async Task PushChooseDice(int diceBet, int userBet)
    {
        var chat = await _chatService.GetChatByIdOrException(_telegramMessageDto.ChatId);
        var gameModel = new GameModel
        {
            Chat = new ChatModel
            {
                Id = _telegramMessageDto.ChatId,
                Balance = chat!.Balance,
                DemoBalance = chat.DemoBalance
            },
            UserBet = userBet,
            IsDemoPlay = true
        };
        var diceGame = new DiceGame(gameModel, _telegramMessageDto.MessageId, _telegramBotClient, 
            _chatService ,diceBet, _serviceProvider);
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
            Chat = new ChatModel
            {
                Id = _telegramMessageDto.ChatId,
                Balance = chat!.Balance,
                DemoBalance = chat.DemoBalance
            },
            UserBet = footballGameParam.UserBet,
            IsDemoPlay = footballGameParam.IsDemoPlay
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
        var currentBalance = _balanceRepository.GetBalanceAsync(_telegramMessageDto.ChatId).ToString();
        _inlineKeyboardButtonsGenerator.InitGetBalanceButtons(payment, currentBalance);
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = _inlineKeyboardButtonsGenerator.ReplyText;

        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId, 
            _replyText, replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushMenuButtonAsync()
    {
        var chat = await _chatRepository.GetChatByIdAsync(_telegramMessageDto.ChatId);
        _inlineKeyboardButtonsGenerator.InitStartButtons(chat!);
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