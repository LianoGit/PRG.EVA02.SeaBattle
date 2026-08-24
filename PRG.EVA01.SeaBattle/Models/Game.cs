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

        public DateTime StartedPlayingOn { get; set; } = DateTime.Now;
        public List<Boat> Boats { get; set; } = new List<Boat>();
        public List<GameLog> GameLogs { get; set; } = new List<GameLog>();
    }
}