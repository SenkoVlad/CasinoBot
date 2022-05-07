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
    private readonly IChatService _chatService;
    protected double WinningsScore;
    
    private readonly IGameResultsRepo _gameResultsRepo;
    private readonly GameParameters _gameParameters;
    private readonly IStringLocalizer<Resources> _localizer;

    protected Game(GameModel gameModel, IServiceProvider serviceProvider)
    {
        _gameModel = gameModel;
        _chatService = serviceProvider.GetRequiredService<IChatService>();
        _gameResultsRepo = serviceProvider.GetRequiredService<IGameResultsRepo>();
        _gameParameters = serviceProvider.GetRequiredService<GameParameters>();
        _localizer = serviceProvider.GetRequiredService<IStringLocalizer<Resources>>();
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

        await Task.Delay(3500);

        _gameModel.DidWin = GetRoundResult();
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

            var bettingResult = _gameParameters.BettingResults.FirstOrDefault(b =>
                b.IsWin == _gameModel.DidWin &&
                b.GameId == _gameModel.GameId);

            if (bettingResult == null)
            {
                throw new Exception("Something wrong, try again");
            }

            await _gameResultsRepo.AddAsync(new GameResult
            {
                ChatId = _gameModel.Chat.Id,
                Bet = _gameModel.UserBet,
                BettingResultId = bettingResult.Id
            });
            WinningsScore = await _chatService.ChangeBalanceAsync(_gameModel, bettingResult);
            transactionScope.Complete();

            return new SaveGameResultModel
            {
                Success = true,
                Message = _localizer[Resources.GameRoundFailed]
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new SaveGameResultModel
            {
                Success = false,
                Message = _localizer[Resources.GameRoundFailed]
            };
        }
    }

    protected abstract Task SendDoNotHaveEnoughMoneyToPlayMessageAsync();
    protected abstract Task InitGameAsync();
    protected abstract Task SentStartMessageAsync();
    protected abstract Task PlayGameRoundAsync();
    protected abstract bool GetRoundResult();
    protected abstract Task SendRoundResultMessageAsync(SaveGameResultModel saveGameResultModel);
}