using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PRG.EVA01.SeaBattle.Models
{
    public class Boat
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        [ValidateNever]
        public Game Game { get; set; } = null!;

        public int LocationId { get; set; }
        [ValidateNever]
        public Location Location { get; set; } = null!;

        public BoatStatus Status { get; set; }
    }
}
