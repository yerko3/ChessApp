using System.Collections.Generic;

namespace chessEngine
{
    public interface IFigure
    {
        Coords Position { get; }
        ColorTypeFigure ColorType { get; }
        FigureType GetFigureType();
        List<Coords> GetAvailablePositions(IChessBoard board);
        bool IsMovimentValid(int x, int y, IChessBoard board);
        void IncrementMoveCount();
    }
}
