using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;
using Currency = Casino.DAL.Models.Currency;

namespace Casino.BLL.Services.Implementation;

public class GameParameters
{
    public Game[] Games { get; }
    public BettingResult[] BettingResults {get;}
    public Currency[] Currencies { get; }

    public GameParameters(IBettingResultsRepo bettingResultsRepo,
        IGamesRepo gamesRepo,
        ICurrenciesRepo currenciesRepo)
    {
        BettingResults = bettingResultsRepo.GetAllBettingResults().ToArray();
        Games = gamesRepo.GetAllGames().ToArray();
        Currencies = currenciesRepo.GetAllCurrencies().ToArray();
    }
}