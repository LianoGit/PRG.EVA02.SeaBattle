using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRG.EVA01.SeaBattle.Services;

namespace PRG.EVA01.SeaBattle.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> ThrowBomb(int gameId)
        {
            var result = await _seaBattleService.PrepareThrowBombAsync(gameId);
            if (result == null)
            {
                return NotFound();
            }

            ApplyThrowBombViewData(result);

            return View(result.Game);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThrowBomb(int gameId, string letter, string number)
        {
            var result = await _seaBattleService.ThrowBombAsync(gameId, letter, number);
            if (result == null)
            {
                return NotFound();
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
