using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRG.EVA01.SeaBattle.Services;

namespace PRG.EVA01.SeaBattle.Controllers
{
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

            var logs = await _dataService.GetGameLogsAsync(gameId);

            return View(logs);
        }

        // GET: GameLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var gameLog = await _dataService.GetGameLogByIdAsync(id.Value);
            if (gameLog == null) return NotFound();

            return View(gameLog);
        }

        // GET: GameLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var gameLog = await _dataService.GetGameLogByIdAsync(id.Value);
            if (gameLog == null) return NotFound();

            return View(gameLog);
        }

        // POST: GameLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dataService.DeleteGameLogAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}