using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRG.EVA01.SeaBattle.Services;

namespace PRG.EVA01.SeaBattle.Controllers
{
    [Authorize(Roles = "Player,Administrator")]
    public class GameLogsController : Controller
    {
        private readonly IDataService _dataService;

        public GameLogsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET: GameLogs
        public async Task<IActionResult> Index(int? gameId)
        {
            if (gameId.HasValue)
            {
                ViewData["GameId"] = gameId.Value;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");
            var logs = await _dataService.GetGameLogsAsync(gameId, userId, isAdmin);

            return View(logs);
        }

        // GET: GameLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");
            var gameLog = await _dataService.GetGameLogByIdAsync(id.Value, userId, isAdmin);
            if (gameLog == null) return Forbid();

            return View(gameLog);
        }

        // GET: GameLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");
            var gameLog = await _dataService.GetGameLogByIdAsync(id.Value, userId, isAdmin);
            if (gameLog == null) return Forbid();

            return View(gameLog);
        }

        // POST: GameLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");
            var deleted = await _dataService.DeleteGameLogAsync(id, userId, isAdmin);
            if (!deleted)
            {
                return Forbid();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}