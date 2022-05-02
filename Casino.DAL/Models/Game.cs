using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casino.DAL.Models;

[Table("dbo.Games")]
public class Game
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}