namespace PRG.EVA01.SeaBattle.Services
{
    public interface ISeaBattleService
    {
        Task<ThrowBombResult?> PrepareThrowBombAsync(int gameId);
        Task<ThrowBombResult?> ThrowBombAsync(int gameId, string? letter, string? number);
    }
}
