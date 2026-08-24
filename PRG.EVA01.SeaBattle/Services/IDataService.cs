using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Services
{
    public interface IDataService
    {
        Task<List<Game>> GetGamesAsync();
        Task<Game> CreateGameWithBoatsAsync(string playerName, int boatCount);

        Task<List<Boat>> GetBoatsAsync();
        Task<Boat?> GetBoatByIdAsync(int id);
        Task<Boat> CreateBoatAsync(Boat boat);
        Task<bool> UpdateBoatAsync(Boat boat);
        Task<bool> DeleteBoatAsync(int id);

        Task<List<Location>> GetLocationsAsync();
        Task<Location?> GetLocationByIdAsync(int id);
        Task<Location> CreateLocationAsync(Location location);
        Task<bool> UpdateLocationAsync(Location location);
        Task<bool> DeleteLocationAsync(int id);

        Task<List<GameLog>> GetGameLogsAsync(int? gameId);
        Task<GameLog?> GetGameLogByIdAsync(int id);
        Task<bool> DeleteGameLogAsync(int id);

        Task<List<Game>> GetGamesForSelectAsync();
        Task<List<Location>> GetLocationsForSelectAsync();
    }
}
