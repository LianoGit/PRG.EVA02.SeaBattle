using System;

namespace PRG.EVA01.SeaBattle.Models
{
    public class GameLog
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string PlayerName { get; set; }
        public string LocationLetter { get; set; }
        public string LocationNumber { get; set; }
        public string Result { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}