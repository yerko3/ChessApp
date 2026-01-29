
using chessEngine;
using User_prueba.Models;

namespace User_prueba
{
    #region user
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Image { get; set; }
    }

    public class MatchInfoUser
    {
        public string MatchName { get; set; }
        public User Owner { get; set; }
        public User Opponent { get; set; }
    }
    #endregion
    public class MatchStatus
    {
        public static readonly MatchStatus Empty = new MatchStatus();
        public long Id { get; set; }
        public string Name { get; set; }
        public long OwnerId { get; set; }
        public long? OpponentId { get; set; }
        public long NextPlayerId { get; set; }
        public long? WinnerId { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
    }


    #region BattleField
    public class BattleField
    {
        public FigureType FigureType { get; set; }
        public ColorTypeFigure ColorType { get; set; }
        public record Figure(int x, int y, ColorTypeFigure Color, FigureType Type);
        public Figure[] Figures { get; set; } = [];

        public MatchStatus Status = MatchStatus.Empty;
    }
    #endregion

    public class AllAvailablePositions
    {
        public class Coords
        {
            public int x;
            public int y;
        }
        public Coords[] AvailableCoords { get; set; } = [];
    }
    public class Figure
    {
        public Guid Id { get; set; }
        public long MatchId { get; set; }
        public short Type { get; set; }    // Ej: Rey, Dama, Torre, etc.
        public short Color { get; set; }   // Ej: 0 = blanco, 1 = negro
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class FigureDesc
    {
        public FigureType Type { get; set; }
        public ColorTypeFigure Color { get; set; }
        public Coords Position { get; set; }
    }

    public interface IRepository
    {
        Task<User> GetUserByIdAsync(int id);
        Task InsertUserAsync(User user);

        Task<User> GetUserByEmailAsync(string email);
        Task<MatchStatus> GetMatchByNameAsync(string matchName);
        Task StartMatchAsync(string matchName);
        Task<MatchStatus[]> GetAllMatchesAsync();
        Task<Match> CreateMatchAsync(string name, long ownerId);
        Task<bool> JoinMatchAsync(string matchName, long opponentId);

        Task<List<Figure>> GetFiguresByMatchIdAsync(long matchId);
        Task<BattleField> GetBattleFieldAsync(string matchName);
        Task MovePieceAsync(string matchName, int fromX, int fromY, int toX, int toY);
        Task<bool> IsInCheckAsync(string matchName, long playerId);
        Task SaveFigures(long matchId, List<FigureDesc> figures);


        Task<AllAvailablePositions.Coords[]> GetValidMovesForPiece(string matchName, int x, int y);
        Task InsertMoveAsync(Move move);
        Task<List<Move>> GetMovesByMatchIdAsync(long matchId);

        Task<bool> IsCheckmateAsync(string matchName, long playerId);

        Task RestartMatchAsync(string matchName);

        Task<bool> DeleteFiguresByMatchIdAsync(long matchId);

        Task<MatchStatus?> GetMatchByUserIdsAsync(int userId, int opponentId);

        Task<MatchInfoUser> GetInfoTwoUserOfMatch(long UserId);
    }
}
