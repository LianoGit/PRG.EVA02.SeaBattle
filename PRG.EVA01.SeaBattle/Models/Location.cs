using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PRG.EVA01.SeaBattle.Models
{
    public class Location
    {
        public int Id { get; set; }

        public string Letter { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        public int GameId { get; set; }
        [ValidateNever]
        public Game Game { get; set; } = null!;

        [ValidateNever]
        public Boat? Boat { get; set; }
    }
}
