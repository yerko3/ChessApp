namespace chessEngine
{
    public class Bishop : Figure
    {
        public Bishop(ColorTypeFigure color, int x, int y) : base(color, x, y)
        {
        }

        public override List<Coords> GetAvailablePositions(IChessBoard board)
        {
            if (board == null)
                return new List<Coords>();
            var result = FigureMovementUtils.GetBishopMoves(this, board);
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
            if (board == null)
                return false;
            var list = FigureMovementUtils.GetBishopMoves(this, board);
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
            return FigureType.BISHOP;
        }
        public override List<Coords> GetThreateningPositions(IChessBoard board)
        {
            var result = FigureMovementUtils.GetBishopMoves(this, board);
            return result;
        }
        public List<Coords> GetThreateningPositions(Coords target, IChessBoard board)
        {
            var result = new List<Coords>();
            int dx = Math.Sign(target.X - Position.X);
            int dy = Math.Sign(target.Y - Position.Y);

            if (Math.Abs(target.X - Position.X) != Math.Abs(target.Y - Position.Y) || (dx == 0 && dy == 0))
                return result; // no es una amenaza válida en diagonal

            int x = Position.X + dx;
            int y = Position.Y + dy;

            while (x != target.X || y != target.Y)
            {
                result.Add(new Coords(x, y));
                x += dx;
                y += dy;
            }

            return result;
        }

    }
}
