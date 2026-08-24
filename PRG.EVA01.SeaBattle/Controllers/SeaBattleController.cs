using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRG.EVA01.SeaBattle.Data;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Controllers
{
    public class SeaBattleController : Controller
    {
        private readonly SeaBattleDbContext _context;
        private static readonly Game _game;

        static SeaBattleController()
        {
            // initialize game using the Game model
            _game = new Game
            {
                Id = 1,
                PlayerName = "Player1",
                StartedPlayingOn = DateTime.Now,
                Boats = _boats
            };
        }

        public SeaBattleController(SeaBattleDbContext context)
        {
            _context = context;
            try
            {
                // only call if we have the default set (avoid calling every request if you prefer)
                if (_boats == null || _boats.Count < 6)
                {
                    LoadBoats().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                // avoid crashing the request pipeline — log so you can see if LoadBoats runs and fails
                Console.WriteLine($"LoadBoats failed in ctor: {ex}");
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ThrowBomb(string letter, string number)
        {
            // Prepare display values
            string displayLetter = letter?.ToUpperInvariant() ?? "";
            string displayNumber = number ?? "";
            ViewData["Location"] = $"{displayLetter} / {displayNumber}";

            // validate letter
            if (string.IsNullOrEmpty(letter) || letter.Length != 1)
            {
                ViewData["ThrowBombMessage"] = "illegale poging";
                ViewData["ThrowBombStatusClass"] = "text-danger";
                ViewData["SunkCount"] = _boats.Count(b => b.Status == BoatStatus.Sunk);
                return View(_game);
            }

            displayLetter = displayLetter.ToUpperInvariant();
            char letterChar = displayLetter[0];
            if (letterChar < 'A' || letterChar > 'T')
            {
                ViewData["ThrowBombMessage"] = "illegale poging";
                ViewData["ThrowBombStatusClass"] = "text-danger";
                ViewData["SunkCount"] = _boats.Count(b => b.Status == BoatStatus.Sunk);
                return View(_game);
            }

            // validate number
            if (string.IsNullOrEmpty(number) || !int.TryParse(number, out int numberValue) || numberValue < 1 || numberValue > 10)
            {
                ViewData["ThrowBombMessage"] = "illegale poging";
                ViewData["ThrowBombStatusClass"] = "text-danger";
                ViewData["SunkCount"] = _boats.Count(b => b.Status == BoatStatus.Sunk);
                return View(_game);
            }

            // normalize number for comparison with stored boats
            number = numberValue.ToString();
            ViewData["Location"] = $"{displayLetter} / {number}";

            foreach (Boat boat in _boats)
            {
                if (boat.Location.Letter == displayLetter && boat.Location.Number == number)
                {
                    boat.Status = BoatStatus.Sunk;
                    ViewData["ThrowBombMessage"] = "HIT!!!";
                    ViewData["ThrowBombStatusClass"] = "text-success";
                    ViewData["SunkCount"] = _boats.Count(b => b.Status == BoatStatus.Sunk);

                    var logHit = new GameLog
                    {
                        GameId = _game.Id,
                        PlayerName = _game.PlayerName,
                        LocationLetter = displayLetter,
                        LocationNumber = number,
                        Result = "HIT",
                        CreatedOn = DateTime.UtcNow
                    };

                    await _context.GameLogs.AddAsync(logHit);
                    await _context.SaveChangesAsync();

                    return View(_game);
                }
            }

            // Miss -> same view as invalid; show result and sunk count
            ViewData["ThrowBombMessage"] = "MISS";
            ViewData["ThrowBombStatusClass"] = "text-warning";
            ViewData["SunkCount"] = _boats.Count(b => b.Status == BoatStatus.Sunk);
            _game.Boats = _boats;

            var logMiss = new GameLog
            {
                GameId = _game.Id,
                PlayerName = _game.PlayerName,
                LocationLetter = displayLetter,
                LocationNumber = number,
                Result = "MISS",
                CreatedOn = DateTime.UtcNow
            };

            await _context.GameLogs.AddAsync(logMiss);
            await _context.SaveChangesAsync();

            return View(_game);
        }

        public async Task LoadBoats()
        {
            const string baseUri = "https://mgp32-api.azurewebsites.net/";
            const string endpoint = "randomlocation/get/6"; // per requirement

            var client = new HttpClient { BaseAddress = new Uri(baseUri) };
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            while (_boats.Count < 6) // give a few tries to get unique locations
            {
                try
                {
                    var json = await client.GetStringAsync(endpoint);
                    var loc = JsonSerializer.Deserialize<Location>(json, options);
                    if (loc == null || string.IsNullOrWhiteSpace(loc.Letter) || string.IsNullOrWhiteSpace(loc.Number))
                        continue;
                    // normalize letter
                    loc.Letter = loc.Letter.ToUpperInvariant();

                    if (_boats.Any(b => string.Equals(b.Location.Letter, loc.Letter, StringComparison.OrdinalIgnoreCase)
                                      && b.Location.Number == loc.Number))
                        continue;

                    await AddBoat(loc);
                }
                catch
                {
                    // ignore transient failures, continue trying
                }
            }
        }

        private async Task<bool> AddBoat(Location location)
        {
            await Task.Yield();
            if (location == null) return false;
            location.Letter = location.Letter?.ToUpperInvariant();

            if (!_boats.Any(b => string.Equals(b.Location.Letter, location.Letter, StringComparison.OrdinalIgnoreCase)
                                      && b.Location.Number == location.Number))
            {
                _boats.Add(new Boat
                {
                    Location = new Location { Letter = location.Letter, Number = location.Number },
                    Status = BoatStatus.Active
                });

                return true;
            }
            else
            {
                return false;
            }
        }

        private static List<Boat> _boats = new List<Boat>
        {
            new Boat
            {
                Location = new Location
                {
                    Letter= "A",
                    Number = "5"
                },
                Status = BoatStatus.Active
            },
            new Boat
            {
                Location = new Location
                {
                    Letter = "C",
                    Number = "7"
                },
                Status = BoatStatus.Active
            },

            new Boat
            {
                Location = new Location
                {
                    Letter = "T",
                    Number = "2"
                },
                Status = BoatStatus.Active
            }
        };
    }
}
