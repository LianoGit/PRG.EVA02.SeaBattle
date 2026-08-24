using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Services
{
    public interface IDataService
    {
        Task<List<Game>> GetGamesAsync(string? userId, bool isAdmin);
        Task<Game?> GetGameByIdAsync(int id, string? userId, bool isAdmin);
        Task<Game> CreateGameWithBoatsAsync(string playerName, string userId, int boatCount);
        Task<bool> UpdateGameNameAsync(int id, string playerName, string? userId, bool isAdmin);

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

        Task<List<GameLog>> GetGameLogsAsync(int? gameId, string? userId, bool isAdmin);
        Task<GameLog?> GetGameLogByIdAsync(int id, string? userId, bool isAdmin);
        Task<bool> DeleteGameLogAsync(int id, string? userId, bool isAdmin);

        Task<List<Game>> GetGamesForSelectAsync();
    }
}
