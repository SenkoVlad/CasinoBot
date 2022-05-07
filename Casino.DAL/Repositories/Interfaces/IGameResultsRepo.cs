using Casino.DAL.Models;

namespace Casino.DAL.Repositories.Interfaces;

public interface IGameResultsRepo
{
    Task AddAsync(GameResult gameResult);
}