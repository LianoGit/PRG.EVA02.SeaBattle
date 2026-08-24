using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Data
{
    public class SeaBattleDbContext : DbContext
    {
        public SeaBattleDbContext(DbContextOptions<SeaBattleDbContext> options) : base(options) { }

        // Mark DbSet as non-nullable to satisfy nullable reference checking
        public DbSet<GameLog> GameLogs { get; set; } = null!;
    }
}