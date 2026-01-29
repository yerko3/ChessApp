
namespace chessEngine
{
    public class Knight : Figure
    {
        public Knight(ColorTypeFigure color, int x, int y) : base(color, x, y)
        {
        }

        public override List<Coords> GetAvailablePositions(IChessBoard board)
        {
            if (board == null)
                return new List<Coords>();
            List<Coords> result = FigureMovementUtils.GetKnightMoves(this, board);
            if (board is ChessBoard chessBoard)
            {
                if (!chessBoard.IsInCheck2(ColorType))
                    return result;
                var king = chessBoard.GetFigure(FigureType.KING, ColorType);
                if (king == null)
                    return new List<Coords>();
                if (chessBoard.CanBlockWithAllies(result) || chessBoard.AlliesCanCaptureThreat(result))
                    return result;
                return new List<Coords>();
            }
            return result;
        }
        public override bool IsMovimentValid(int x, int y, IChessBoard board)
        {
            var list = FigureMovementUtils.GetKnightMoves(this, board);
            if (list.Count == 0)
                return false;

            for (int i = 0; i < list.Count; i++)
            {
                if (x == list[i].X && y == list[i].Y)
                    return true;
            }
            return false;
        }
        public override FigureType GetFigureType()
        {
            return FigureType.KNIGHT;
        }

        public override List<Coords> GetThreateningPositions(IChessBoard board)
        {
            if (board == null)
                return new List<Coords>();
            List<Coords> result = FigureMovementUtils.GetKnightMoves(this, board);
            return result;
        }
    }
}
