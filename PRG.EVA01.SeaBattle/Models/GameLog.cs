using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PRG.EVA01.SeaBattle.Models
{
    public class GameLog
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        [ValidateNever]
        public Game Game { get; set; } = null!;

        public string PlayerName { get; set; } = string.Empty;
        public string LocationLetter { get; set; } = string.Empty;
        public string LocationNumber { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}