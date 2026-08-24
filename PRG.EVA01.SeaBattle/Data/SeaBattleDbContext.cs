using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Data
{
    public class SeaBattleDbContext : DbContext
    {
        public SeaBattleDbContext(DbContextOptions<SeaBattleDbContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Boat> Boats { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<GameLog> GameLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>()
                .Property(g => g.PlayerName)
                .HasColumnName("GameName")
                .HasMaxLength(100);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Boats)
                .WithOne(b => b.Game)
                .HasForeignKey(b => b.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.GameLogs)
                .WithOne(gl => gl.Game)
                .HasForeignKey(gl => gl.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Location>()
                .HasOne(l => l.Game)
                .WithMany()
                .HasForeignKey(l => l.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Boat>()
                .HasOne(b => b.Location)
                .WithOne(l => l.Boat)
                .HasForeignKey<Boat>(b => b.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Location>()
                .HasIndex(l => new { l.GameId, l.Letter, l.Number })
                .IsUnique();
        }
    }
}