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
                var userFootballBet = JsonConvert.DeserializeObject<int>(commandDto.CommandParamJson);
                await PushHitBallButtonAsync(userFootballBet);
                break;
            case Command.ChooseDiceGameButtonCommand:
                await PushDiceButtonAsync();
                break;
            case Command.DemoPlayFootballButtonCommand:
                await PushFootballDemoPlayButtonAsync();
                break;
            case Command.MakeBetButtonCommand:
                var diceGameParamDto = JsonConvert.DeserializeObject<DiceGameParamDto>(commandDto.CommandParamJson);
                await PushChooseDice(diceGameParamDto.Dice, diceGameParamDto.UserBet);
                break;
            case Command.IncreaseBalancePayment:
            case Command.DecreaseBalancePayment:
                var newPayment = JsonConvert.DeserializeObject<int>(commandDto.CommandParamJson);
                await PushGetBalanceButtonAsync(newPayment);
                break;
            case Command.IncreaseDiceBetPayment:
            case Command.DecreaseDiceBetPayment:
                var newDiceBet = JsonConvert.DeserializeObject<int>(commandDto.CommandParamJson);
                await PushDiceButtonAsync(newDiceBet);
                break;
            case Command.IncreaseFootballBetPayment:
            case Command.DecreaseFootballBetPayment:
                var newFootballBet = JsonConvert.DeserializeObject<int>(commandDto.CommandParamJson);
                await PushFootballDemoPlayButtonAsync(newFootballBet);
                break;
            case Command.DepositPayment:
                var deposit = JsonConvert.DeserializeObject<int>(commandDto.CommandParamJson);
                await PushDepositButtonAsync(deposit);
                break;
            case Command.SwitchLanguageCommand:
                var language = commandDto.CommandParamJson;
                await PushSwitchLanguageAsync(language!);
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
        _inlineKeyboardButtonsGenerator.InitSettingsButtons();
        _replyText = string.Concat(
            _localizer[Resources.CurrentLanguageButtonText],
            currentLanguage);
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
        _replyText = _localizer[Resources.ChooseGameModeMessageText];
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushChooseDice(int diceBet, int userBet)
    {
        var gameModel = new GameModel
        {
            ChatId = _telegramMessageDto.ChatId,
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
        _inlineKeyboardButtonsGenerator.InitStartButtons();
        _replyText = string.Concat(
            _localizer[Resources.GetMyBalanceMessageText, chat.Balance],
            Environment.NewLine,
             _localizer[Resources.ChooseActionMessageText]);
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.SendTextMessageAsync(_telegramMessageDto.ChatId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushDiceButtonAsync(int userDiceBet = AppConstants.DefaultUserBet)
    {
        _inlineKeyboardButtonsGenerator.InitDiceChooseBetButtons(userDiceBet);
        _replyText = _localizer[Resources.ChooseYourBetMessageText];
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    } 

    private async Task PushHitBallButtonAsync(int userBet)
    {
        var gameModel = new GameModel
        {
            ChatId = _telegramMessageDto.ChatId,
            UserBet = userBet,
            IsDemoPlay = true
        };
        var footBallGame = new FootBallGame(gameModel, _telegramMessageDto.MessageId,
            _telegramBotClient, _chatService, _serviceProvider);
        await footBallGame.PlayRoundAsync();
    }

    private async Task PushFootballDemoPlayButtonAsync(int userBet = AppConstants.DefaultUserBet)
    {
        _inlineKeyboardButtonsGenerator.InitPlayFootballDemoButtons(userBet);
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        _replyText = $"{Resources.FootballGameButtonText}"; await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId,
            _telegramMessageDto.MessageId, 
            text: _replyText,replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushGetBalanceButtonAsync(int payment = AppConstants.DefaultBalancePayment)
    {
        _inlineKeyboardButtonsGenerator.InitGetBalanceButtons(payment);
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;
        var currentBalance = _balanceRepository.GetBalanceAsync(_telegramMessageDto.ChatId).ToString();
        _replyText = _localizer[Resources.GetMyBalanceMessageText, currentBalance];

        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId, 
            _replyText, replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushMenuButtonAsync()
    {
        var chat = await _chatRepository.GetChatByIdAsync(_telegramMessageDto.ChatId);
        _inlineKeyboardButtonsGenerator.InitStartButtons();
        _replyText = string.Concat(
            _localizer[Resources.GetMyBalanceMessageText, chat!.Balance],
            Environment.NewLine,
            _localizer[Resources.ChooseActionMessageText]);
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId, _replyText,
            replyMarkup: _inlineKeyboardButtons);
    }

    private async Task PushGamesButtonAsync()
    {
        _inlineKeyboardButtonsGenerator.InitGamesButtons();
        _replyText = _localizer[Resources.ChooseActionMessageText];
        _inlineKeyboardButtons = _inlineKeyboardButtonsGenerator.GetInlineKeyboardMarkup;

        await _telegramBotClient.EditMessageTextAsync(_telegramMessageDto.ChatId, _telegramMessageDto.MessageId,
            _localizer[Resources.ChooseActionMessageText], replyMarkup: _inlineKeyboardButtons);
    }
}