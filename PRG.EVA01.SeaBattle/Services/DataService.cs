using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Data;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Services
{
    public class DataService : IDataService
    {
        private readonly SeaBattleDbContext _context;
        private readonly HttpClient _httpClient;

        public DataService(SeaBattleDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient { BaseAddress = new Uri("https://mgp32-api.azurewebsites.net/") };
        }

        public async Task<List<Game>> GetGamesAsync(string? userId, bool isAdmin)
        {
            var query = _context.Games
                .Include(g => g.Boats)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(g => g.UserId == userId);
            }

            return await query
                .OrderByDescending(g => g.StartedPlayingOn)
                .ToListAsync();
        }

        public async Task<Game?> GetGameByIdAsync(int id, string? userId, bool isAdmin)
        {
            var query = _context.Games
                .Include(g => g.Boats)
                .ThenInclude(b => b.Location)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(g => g.UserId == userId);
            }

            return await query.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Game> CreateGameWithBoatsAsync(string playerName, string userId, int boatCount)
        {
            var game = new Game
            {
                PlayerName = playerName,
                UserId = userId,
                StartedPlayingOn = DateTime.Now
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            await CreateBoatsForGameAsync(game.Id, boatCount);
            return game;
        }

        public async Task<bool> UpdateGameNameAsync(int id, string playerName, string? userId, bool isAdmin)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return false;
            }

            if (!isAdmin && game.UserId != userId)
            {
                return false;
            }

            game.PlayerName = playerName;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Boat>> GetBoatsAsync()
        {
            return await _context.Boats
                .Include(b => b.Game)
                .Include(b => b.Location)
                .OrderBy(b => b.GameId)
                .ThenBy(b => b.Id)
                .ToListAsync();
        }

        public async Task<Boat?> GetBoatByIdAsync(int id)
        {
            return await _context.Boats
                .Include(b => b.Game)
                .Include(b => b.Location)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Boat> CreateBoatAsync(Boat boat)
        {
            _context.Boats.Add(boat);
            await _context.SaveChangesAsync();
            return boat;
        }

        public async Task<bool> UpdateBoatAsync(Boat boat)
        {
            if (!await _context.Boats.AnyAsync(b => b.Id == boat.Id))
            {
                return false;
            }

            _context.Boats.Update(boat);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBoatAsync(int id)
        {
            var boat = await _context.Boats.FindAsync(id);
            if (boat == null)
            {
                return false;
            }

            _context.Boats.Remove(boat);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Location>> GetLocationsAsync()
        {
            return await _context.Locations
                .Include(l => l.Game)
                .Include(l => l.Boat)
                .OrderBy(l => l.GameId)
                .ThenBy(l => l.Letter)
                .ThenBy(l => l.Number)
                .ToListAsync();
        }

        public async Task<Location?> GetLocationByIdAsync(int id)
        {
            return await _context.Locations
                .Include(l => l.Game)
                .Include(l => l.Boat)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Location> CreateLocationAsync(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<bool> UpdateLocationAsync(Location location)
        {
            if (!await _context.Locations.AnyAsync(l => l.Id == location.Id))
            {
                return false;
            }

            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return false;
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<GameLog>> GetGameLogsAsync(int? gameId, string? userId, bool isAdmin)
        {
            var query = _context.GameLogs
                .Include(gl => gl.Game)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(gl => gl.Game.UserId == userId);
            }

            if (gameId.HasValue)
            {
                query = query.Where(gl => gl.GameId == gameId.Value);
            }

            return await query
                .OrderByDescending(gl => gl.CreatedOn)
                .ToListAsync();
        }

        public async Task<GameLog?> GetGameLogByIdAsync(int id, string? userId, bool isAdmin)
        {
            var query = _context.GameLogs
                .Include(gl => gl.Game)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(gl => gl.Game.UserId == userId);
            }

            return await query.FirstOrDefaultAsync(gl => gl.Id == id);
        }

        public async Task<bool> DeleteGameLogAsync(int id, string? userId, bool isAdmin)
        {
            var query = _context.GameLogs
                .Include(gl => gl.Game)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(gl => gl.Game.UserId == userId);
            }

            var gameLog = await query.FirstOrDefaultAsync(gl => gl.Id == id);
            if (gameLog == null)
            {
                return false;
            }

            _context.GameLogs.Remove(gameLog);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Game>> GetGamesForSelectAsync()
        {
            return await _context.Games.OrderBy(g => g.Id).ToListAsync();
        }

        private async Task CreateBoatsForGameAsync(int gameId, int amount)
        {
            const string endpoint = "randomlocation/get/6";
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var usedLocations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var created = 0;
            var attempts = 0;

            while (created < amount && attempts < 100)
            {
                attempts++;
                try
                {
                    var json = await _httpClient.GetStringAsync(endpoint);
                    var apiLocation = JsonSerializer.Deserialize<Location>(json, options);

                    if (apiLocation == null ||
                        string.IsNullOrWhiteSpace(apiLocation.Letter) ||
                        string.IsNullOrWhiteSpace(apiLocation.Number))
                    {
                        continue;
                    }

                    var letter = apiLocation.Letter.ToUpperInvariant();
                    var number = apiLocation.Number.Trim();
                    var key = $"{letter}:{number}";

                    if (!usedLocations.Add(key))
                    {
                        continue;
                    }

                    var location = new Location
                    {
                        GameId = gameId,
                        Letter = letter,
                        Number = number
                    };

                    var boat = new Boat
                    {
                        GameId = gameId,
                        Location = location,
                        Status = BoatStatus.Active
                    };

                    _context.Boats.Add(boat);
                    created++;
                }
                catch
                {
                    // api is flaky sometimes, just try again
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
