using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.Services;

namespace PRG.EVA01.SeaBattle.Controllers
{
    public class LocationsController : Controller
    {
        private readonly IDataService _dataService;

        public LocationsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IActionResult> Index()
        {
            var locations = await _dataService.GetLocationsAsync();
            return View(locations);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _dataService.GetLocationByIdAsync(id.Value);

            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateGameSelectionsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Letter,Number,GameId")] Location location)
        {
            if (ModelState.IsValid)
            {
                await _dataService.CreateLocationAsync(location);
                return RedirectToAction(nameof(Index));
            }

            await PopulateGameSelectionsAsync(location.GameId);
            return View(location);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _dataService.GetLocationByIdAsync(id.Value);
            if (location == null)
            {
                return NotFound();
            }

            await PopulateGameSelectionsAsync(location.GameId);
            return View(location);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Letter,Number,GameId")] Location location)
        {
            if (id != location.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updated = await _dataService.UpdateLocationAsync(location);
                if (!updated)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            await PopulateGameSelectionsAsync(location.GameId);
            return View(location);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _dataService.GetLocationByIdAsync(id.Value);

            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dataService.DeleteLocationAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateGameSelectionsAsync(int? selectedGameId = null)
        {
            var games = await _dataService.GetGamesForSelectAsync();
            ViewData["GameId"] = new SelectList(games, "Id", "Id", selectedGameId);
        }
    }
}
