using System.Collections.Generic;

namespace chessEngine
{
    public class King : Figure
    {
        public delegate bool FilterDelegateKing(Coords pos);
        public King(ColorTypeFigure color, int x, int y) : base(color, x, y)
        {
        }

        public override List<Coords> GetAvailablePositions(IChessBoard board)
        {
            if (board == null)
                return new List<Coords>();
            var result = FigureMovementUtils.GetKingMoves(this, board);
            var currentY = this.Position.Y;
            if (board is ChessBoard cb)
            {
                if (cb == null)
                    return result;
                if (cb.CanCastleKingSide(this))
                {
                    Console.WriteLine("Enroque corto permitido");
                    result.Add(new Coords(6, currentY)); // g1 o g8
                }
                if (cb.CanCastleQueenSide(this))
                {
                    Console.WriteLine("Enroque largo permitido");
                    result.Add(new Coords(2, currentY)); // c1 o c8
                }
                if (!cb.IsInCheck2(ColorType)) // ojo aqui `porque cuando esta puede ser que no este en check pero unas de mis posciones si que lo este 
                    return result;
                List<Coords> movimientosSeguros = new List<Coords>();
                for (int i = 0; i < result.Count; i++)
                {
                    var pos = result[i];
                    if (!cb.IsPositionUnderThreat(this, pos))
                    {
                        movimientosSeguros.Add(pos);
                    }
                }
                return movimientosSeguros;
            }
            return result;
        }
        //var filter = k.GetAvailablePositionsFilter(board, pos => !board.IsPositionUnderThreat((King)king, pos));
        // posiciones que amenazan al rey
        public List<Coords> GetAvailablePositionsFilter(IChessBoard board, FilterDelegateKing where)
        {
            if (where == null)
                return new List<Coords>();
            List<Coords> result = new List<Coords>();
            List<Coords> list = GetAvailablePositions(board);
            for (int i = 0; i < list.Count; i++)
            {
                if (where(list[i]))
                    result.Add(list[i]);
            }
            return result;
        }
        public override bool IsMovimentValid(int x, int y, IChessBoard board)
        {
            if (board == null)
                return false;
            var list = FigureMovementUtils.GetKingMoves(this, board);
            if (board.CanCastleQueenSide(this))
                list.Add(new Coords(2, this.Position.Y)); // c1 o c8
            if (board.CanCastleKingSide(this))
                list.Add(new Coords(6, this.Position.Y)); // g1 o g8
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
            return FigureType.KING;
        }

        public override List<Coords> GetThreateningPositions(IChessBoard b)
        {
            if (b == null)
                return new List<Coords>();
            var result = FigureMovementUtils.GetKingMoves(this, b);
            return result;
        }
    }
}
