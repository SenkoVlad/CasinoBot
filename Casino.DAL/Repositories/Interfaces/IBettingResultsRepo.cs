using Casino.DAL.Models;

namespace Casino.DAL.Repositories.Interfaces;

public interface IBettingResultsRepo
{
    IEnumerable<BettingResult> GetAllBettingResults();
}