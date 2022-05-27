using System.Transactions;
using Casino.BLL.Models;
using Casino.BLL.Services.Implementation;
using Casino.BLL.Services.Interfaces;
using Casino.Common.AppConstants;
using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Casino.BLL.Games;

public abstract class Game
{
    private readonly GameModel _gameModel;
    private readonly int _delayAfterRound;
    private readonly IGameResultsRepo _gameResultsRepo;
    
    protected readonly IStringLocalizer<Resources> Localizer;
    protected readonly IChatService ChatService;
    protected double WinningsScore;
    protected readonly GameParameters GameParameters;

    protected Game(GameModel gameModel, IServiceProvider serviceProvider, int delayAfterRound)
    {
        _gameModel = gameModel;
        _delayAfterRound = delayAfterRound;
        ChatService = serviceProvider.GetRequiredService<IChatService>();
        _gameResultsRepo = serviceProvider.GetRequiredService<IGameResultsRepo>();
        GameParameters = serviceProvider.GetRequiredService<GameParameters>();
        Localizer = serviceProvider.GetRequiredService<IStringLocalizer<Resources>>();
    }

    public virtual async Task PlayRoundAsync()
    {
        if (!IsMoneyEnoughToPlay())
        {
            await SendDoNotHaveEnoughMoneyToPlayMessageAsync();
            await InitGameAsync();
            return;
        }

        await SentStartMessageAsync();
        await PlayGameRoundAsync();

        await Task.Delay(_delayAfterRound);

        SetRoundResult();
        var saveGameResult = await SaveGameResultAsync();
        await SendRoundResultMessageAsync(saveGameResult);

        await InitGameAsync();
    }

    private bool IsMoneyEnoughToPlay()
    {
        var isBalanceEnoughToPlay = _gameModel.IsDemoPlay
            ? _gameModel.Chat.DemoBalance >= _gameModel.UserBet
            : _gameModel.Chat.Balance >= _gameModel.UserBet;

        return isBalanceEnoughToPlay;
    }


    public virtual async Task<SaveGameResultModel> SaveGameResultAsync()
    {
        try
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (_gameModel.BettingResult == null)
            {
                throw new Exception("Something wrong, try again");
            }

            await _gameResultsRepo.AddAsync(new GameResult
            {
                ChatId = _gameModel.Chat.Id,
                Bet = _gameModel.UserBet,
                BettingResultId = _gameModel.BettingResult.Id
            });
            WinningsScore = await ChatService.ChangeBalanceAsync(_gameModel);
            transactionScope.Complete();

            return new SaveGameResultModel
            {
                Success = true,
                Message = Localizer[Resources.GameRoundFailed]
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new SaveGameResultModel
            {
                Success = false,
                Message = Localizer[Resources.GameRoundFailed]
            };
        }
    }

    protected virtual void SetRoundResult()
    {
        var bettingWinResult = GameParameters.BettingResults.FirstOrDefault(b => 
            b.DiceResult == _gameModel.DiceResult &&
            b.GameId == _gameModel.GameId &&
            b.IsWon);

        var bettingLostResult = GameParameters.BettingResults.FirstOrDefault(b =>
            b.GameId == _gameModel.GameId &&
            !b.IsWon);

        var bettingResult = bettingWinResult ?? bettingLostResult;

        if (bettingResult == null)
        {
            throw new Exception($"{nameof(SetRoundResult)} bettingResult was not found");
        }

        _gameModel.BettingResult = new BettingResultModel
        {
            Id = bettingResult.Id,
            Coefficient = bettingResult.Coefficient,
            IsWon = bettingResult.IsWon
        };
    }

    protected abstract Task SendDoNotHaveEnoughMoneyToPlayMessageAsync();
    protected abstract Task InitGameAsync();
    protected abstract Task SentStartMessageAsync();
    protected abstract Task PlayGameRoundAsync();
    protected abstract Task SendRoundResultMessageAsync(SaveGameResultModel saveGameResultModel);
}