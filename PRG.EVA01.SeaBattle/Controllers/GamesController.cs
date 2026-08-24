using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.Services;

namespace PRG.EVA01.SeaBattle.Controllers
{
    public class GamesController : Controller
    {
        private readonly IDataService _dataService;

        public GamesController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _dataService.GetGamesAsync();
            return View(games);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Game());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlayerName")] Game game)
        {
            if (string.IsNullOrWhiteSpace(game.PlayerName))
            {
                ModelState.AddModelError(nameof(Game.PlayerName), "PlayerName is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(game);
            }

            var createdGame = await _dataService.CreateGameWithBoatsAsync(game.PlayerName, 6);
            return RedirectToAction("ThrowBomb", "SeaBattle", new { gameId = createdGame.Id });
        }
    }
}
