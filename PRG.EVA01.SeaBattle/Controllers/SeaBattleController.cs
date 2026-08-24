using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRG.EVA01.SeaBattle.Services;

namespace PRG.EVA01.SeaBattle.Controllers
{
    [Authorize(Roles = "Player,Administrator")]
    public class SeaBattleController : Controller
    {
        private readonly ISeaBattleService _seaBattleService;

        public SeaBattleController(ISeaBattleService seaBattleService)
        {
            _seaBattleService = seaBattleService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Games");
        }


// Get the game and check if the user is allowed to play it, if not return forbid
        [HttpGet]
        public async Task<IActionResult> ThrowBomb(int gameId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");
            var result = await _seaBattleService.PrepareThrowBombAsync(gameId, userId, isAdmin);
            if (result == null)
            {
                return Forbid();
            }

            ApplyThrowBombViewData(result);

            return View(result.Game);
        }

// Post the throw bomb action, check if the user is allowed to play it, if not return forbid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThrowBomb(int gameId, string letter, string number)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");
            var result = await _seaBattleService.ThrowBombAsync(gameId, letter, number, userId, isAdmin);
            if (result == null)
            {
                return Forbid();
            }

            ApplyThrowBombViewData(result);
            return View(result.Game);
        }

        private void ApplyThrowBombViewData(ThrowBombResult result)
        {
            ViewData["Location"] = result.Location;
            ViewData["ThrowBombMessage"] = result.Message;
            ViewData["ThrowBombStatusClass"] = result.StatusClass;
            ViewData["SunkCount"] = result.SunkCount;
        }
    }
}
