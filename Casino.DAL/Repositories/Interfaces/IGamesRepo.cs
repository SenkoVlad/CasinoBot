using Casino.DAL.Models;

namespace Casino.DAL.Repositories.Interfaces;

public interface IGamesRepo
{
    IEnumerable<Game> GetAllGames();
}