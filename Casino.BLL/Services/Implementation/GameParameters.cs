using Casino.DAL.Models;
using Casino.DAL.Repositories.Interfaces;

namespace Casino.BLL.Services.Implementation;

public class GameParameters
{
    public Game[] Games { get; }
    public BettingResult[] BettingResults {get;}

    public GameParameters(IBettingResultsRepo bettingResultsRepo,
        IGamesRepo gamesRepo)
    {
        BettingResults = bettingResultsRepo.GetAllBettingResults().ToArray();
        Games = gamesRepo.GetAllGames().ToArray();
    }
}