using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.Services;

namespace PRG.EVA01.SeaBattle.Controllers
{
    [Authorize(Roles = "Player,Administrator")]
    public class GamesController : Controller
    {
        private readonly IDataService _dataService;

        public GamesController(IDataService dataService)
        {
            _dataService = dataService;
        }

// Get all games for the user, if the user is an admin, get all games
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");
            var games = await _dataService.GetGamesAsync(userId, isAdmin);
            return View(games);
        }

// Get the game by id, if the user is not allowed to see it, return forbid sent to PAGE
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");
            var game = await _dataService.GetGameByIdAsync(id, userId, isAdmin);
            if (game == null)
            {
                return Forbid();
            }

            return View(game);
        }

// Get the game by id, if the user is not allowed to see it, return forbid SENT TO INPUTS
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");
            var game = await _dataService.GetGameByIdAsync(id, userId, isAdmin);
            if (game == null)
            {
                return Forbid();
            }

            return View(game);
        }

// Post the edit action, if the user is not allowed to edit it, return forbid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlayerName")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(game.PlayerName))
            {
                ModelState.AddModelError(nameof(Game.PlayerName), "PlayerName is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(game);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");
            var updated = await _dataService.UpdateGameNameAsync(id, game.PlayerName, userId, isAdmin);
            if (!updated)
            {
                return Forbid();
            }

            return RedirectToAction(nameof(Index));
        }

// create new game
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Game());
        }

// Post the create action, if the user is not allowed to create it, return challenge
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var createdGame = await _dataService.CreateGameWithBoatsAsync(game.PlayerName, userId, 6);
            return RedirectToAction("ThrowBomb", "SeaBattle", new { gameId = createdGame.Id });
        }
    }
}
