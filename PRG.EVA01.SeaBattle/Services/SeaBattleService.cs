using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Data;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Services
{
    public class SeaBattleService : ISeaBattleService
    {
        private readonly SeaBattleDbContext _context;

        public SeaBattleService(SeaBattleDbContext context)
        {
            _context = context;
        }

        public async Task<ThrowBombResult?> PrepareThrowBombAsync(int gameId, string? userId, bool isAdmin)
        {
            var game = await GetGameAsync(gameId, userId, isAdmin);
            if (game == null)
            {
                return null;
            }

            return new ThrowBombResult
            {
                Game = game,
                Location = "-",
                Message = "Geef een locatie in om te starten.",
                StatusClass = "text-muted",
                SunkCount = game.Boats.Count(b => b.Status == BoatStatus.Sunk)
            };
        }

        public async Task<ThrowBombResult?> ThrowBombAsync(int gameId, string? letter, string? number, string? userId, bool isAdmin)
        {
            var game = await GetGameAsync(gameId, userId, isAdmin);
            if (game == null)
            {
                return null;
            }

            var displayLetter = letter?.ToUpperInvariant() ?? string.Empty;
            var displayNumber = number ?? string.Empty;

            var baseResult = new ThrowBombResult
            {
                Game = game,
                Location = $"{displayLetter} / {displayNumber}"
            };

            if (!IsValidLetter(displayLetter))
            {
                return SetIllegalAttempt(baseResult);
            }

            if (!IsValidNumber(number, out var numberValue))
            {
                return SetIllegalAttempt(baseResult);
            }

            var normalizedNumber = numberValue.ToString();
            baseResult.Location = $"{displayLetter} / {normalizedNumber}";

            var hitBoat = game.Boats.FirstOrDefault(boat =>
                boat.Location.Letter == displayLetter &&
                boat.Location.Number == normalizedNumber &&
                boat.Status == BoatStatus.Active);

            string result;
            if (hitBoat != null)
            {
                hitBoat.Status = BoatStatus.Sunk;
                result = "HIT";
                baseResult.Message = "HIT!!!";
                baseResult.StatusClass = "text-success";
            }
            else
            {
                result = "MISS";
                baseResult.Message = "MISS";
                baseResult.StatusClass = "text-warning";
            }

            var log = new GameLog
            {
                GameId = game.Id,
                PlayerName = game.PlayerName,
                LocationLetter = displayLetter,
                LocationNumber = normalizedNumber,
                Result = result,
                CreatedOn = DateTime.UtcNow
            };

            await _context.GameLogs.AddAsync(log);
            await _context.SaveChangesAsync();

            baseResult.SunkCount = game.Boats.Count(b => b.Status == BoatStatus.Sunk);
            return baseResult;
        }

        private async Task<Game?> GetGameAsync(int gameId, string? userId, bool isAdmin)
        {
            var query = _context.Games
                .Include(g => g.Boats)
                .ThenInclude(b => b.Location)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(g => g.UserId == userId);
            }

            return await query.FirstOrDefaultAsync(g => g.Id == gameId);
        }

        private static ThrowBombResult SetIllegalAttempt(ThrowBombResult result)
        {
            result.Message = "illegale poging";
            result.StatusClass = "text-danger";
            result.SunkCount = result.Game.Boats.Count(b => b.Status == BoatStatus.Sunk);
            return result;
        }

        private static bool IsValidLetter(string? letter)
        {
            if (string.IsNullOrEmpty(letter) || letter.Length != 1)
            {
                return false;
            }

            var letterChar = letter[0];
            return letterChar >= 'A' && letterChar <= 'T';
        }

        private static bool IsValidNumber(string? number, out int numberValue)
        {
            numberValue = 0;

            if (string.IsNullOrEmpty(number) || !int.TryParse(number, out numberValue))
            {
                return false;
            }

            return numberValue >= 1 && numberValue <= 10;
        }
    }
}
