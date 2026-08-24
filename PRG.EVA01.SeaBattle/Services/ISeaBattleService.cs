namespace PRG.EVA01.SeaBattle.Services
{
    public interface ISeaBattleService
    {
        Task<ThrowBombResult?> PrepareThrowBombAsync(int gameId, string? userId, bool isAdmin);
        Task<ThrowBombResult?> ThrowBombAsync(int gameId, string? letter, string? number, string? userId, bool isAdmin);
    }
}
