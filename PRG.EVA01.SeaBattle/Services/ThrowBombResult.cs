using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Services
{
    public class ThrowBombResult
    {
        public Game Game { get; set; } = null!;
        public string Location { get; set; } = "-";
        public string Message { get; set; } = string.Empty;
        public string StatusClass { get; set; } = "text-muted";
        public int SunkCount { get; set; }
    }
}
