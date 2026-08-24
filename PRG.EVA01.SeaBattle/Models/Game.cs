using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRG.EVA01.SeaBattle.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        [Column("GameName")]
        public string PlayerName { get; set; } = string.Empty;

        // Links a game to the authenticated user for ownership checks.
        public string? UserId { get; set; }

        public DateTime StartedPlayingOn { get; set; } = DateTime.Now;
        public List<Boat> Boats { get; set; } = new List<Boat>();
        public List<GameLog> GameLogs { get; set; } = new List<GameLog>();
    }
}