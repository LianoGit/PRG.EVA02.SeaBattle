using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.Services;

namespace PRG.EVA01.SeaBattle.Controllers
{
    public class BoatsController : Controller
    {
        private readonly IDataService _dataService;

        public BoatsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IActionResult> Index()
        {
            var boats = await _dataService.GetBoatsAsync();
            return View(boats);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _dataService.GetBoatByIdAsync(id.Value);

            if (boat == null)
            {
                return NotFound();
            }

            return View(boat);
        }

        public async Task<IActionResult> Create()
        {
            var games = await _dataService.GetGamesForSelectAsync();
            if (!games.Any())
            {
                TempData["CreateBoatMessage"] = "Maak eerst een Game aan voor je een Boat maakt.";
                return RedirectToAction("Create", "Games");
            }

            var hasAvailableLocations = await PopulateSelectionsAsync();
            if (!hasAvailableLocations)
            {
                TempData["CreateBoatMessage"] = "Er zijn geen vrije locaties beschikbaar. Maak eerst een nieuwe Location aan.";
                return RedirectToAction("Create", "Locations");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GameId,LocationId,Status")] Boat boat)
        {
            var games = await _dataService.GetGamesForSelectAsync();
            if (!games.Any())
            {
                TempData["CreateBoatMessage"] = "Maak eerst een Game aan voor je een Boat maakt.";
                return RedirectToAction("Create", "Games");
            }

            var availableLocations = await _dataService.GetLocationsAsync();
            var freeLocations = availableLocations.Where(l => l.Boat == null).ToList();

            if (!freeLocations.Any())
            {
                TempData["CreateBoatMessage"] = "Er zijn geen vrije locaties beschikbaar. Maak eerst een nieuwe Location aan.";
                return RedirectToAction("Create", "Locations");
            }

            var selectedLocationIsAvailable = freeLocations.Any(l => l.Id == boat.LocationId);

            if (!selectedLocationIsAvailable)
            {
                ModelState.AddModelError(nameof(Boat.LocationId), "Kies een vrije locatie.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _dataService.CreateBoatAsync(boat);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "De gekozen locatie is al in gebruik door een andere boot.");
                }
            }

            await PopulateSelectionsAsync(boat.GameId, boat.LocationId);
            return View(boat);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _dataService.GetBoatByIdAsync(id.Value);
            if (boat == null)
            {
                return NotFound();
            }

            await PopulateSelectionsAsync(boat.GameId, boat.LocationId, boat.Id);
            return View(boat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GameId,LocationId,Status")] Boat boat)
        {
            if (id != boat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var updated = await _dataService.UpdateBoatAsync(boat);
                    if (!updated)
                    {
                        return NotFound();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "De gekozen locatie is al in gebruik door een andere boot.");
                }
            }

            await PopulateSelectionsAsync(boat.GameId, boat.LocationId, boat.Id);
            return View(boat);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _dataService.GetBoatByIdAsync(id.Value);

            if (boat == null)
            {
                return NotFound();
            }

            return View(boat);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dataService.DeleteBoatAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PopulateSelectionsAsync(int? selectedGameId = null, int? selectedLocationId = null, int? currentBoatId = null)
        {
            var games = await _dataService.GetGamesForSelectAsync();
            var locations = await _dataService.GetLocationsAsync();

            var availableLocations = locations
                .Where(l => l.Boat == null || (currentBoatId.HasValue && l.Boat != null && l.Boat.Id == currentBoatId.Value))
                .OrderBy(l => l.Id)
                .ToList();

            ViewData["GameId"] = new SelectList(games, "Id", "Id", selectedGameId);
            ViewData["LocationId"] = new SelectList(availableLocations, "Id", "Id", selectedLocationId);

            return availableLocations.Count > 0;
        }
    }
}
