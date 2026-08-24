using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Data;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Controllers
{
    public class GameLogsController : Controller
    {
        private readonly SeaBattleDbContext _context;

        public GameLogsController(SeaBattleDbContext context)
        {
            _context = context;
        }

        // GET: GameLogs
        public async Task<IActionResult> Index()
        {
            var logs = await _context.GameLogs.ToListAsync();
            return View(logs);
        }

        // GET: GameLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var gameLog = await _context.GameLogs.FirstOrDefaultAsync(m => m.Id == id);
            if (gameLog == null) return NotFound();

            return View(gameLog);
        }

        // GET: GameLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var gameLog = await _context.GameLogs.FirstOrDefaultAsync(m => m.Id == id);
            if (gameLog == null) return NotFound();

            return View(gameLog);
        }

        // POST: GameLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameLog = await _context.GameLogs.FindAsync(id);
            if (gameLog != null)
            {
                _context.GameLogs.Remove(gameLog);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}