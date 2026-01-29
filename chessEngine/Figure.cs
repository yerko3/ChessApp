using System.Collections.Generic;

namespace chessEngine
{
    public enum ColorTypeFigure
    {
        BLACK, WHITE
    }
    public enum FigureType
    {
        KING, QUEEN, ROOK, BISHOP, KNIGHT, PAWN, NONE
    }

    public abstract class Figure : IFigure
    {
        private int _moveCount = 0;
        public int MoveCount => _moveCount;
        public bool HasMove => _moveCount > 0;

        protected ColorTypeFigure _typeColor;
        public ColorTypeFigure ColorType => _typeColor;
        public abstract FigureType GetFigureType();
        public abstract List<Coords> GetAvailablePositions(IChessBoard board);
        public abstract bool IsMovimentValid(int x, int y, IChessBoard board);
        public abstract List<Coords> GetThreateningPositions(IChessBoard board);


        private Coords _position;


        public Coords Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }


        protected Figure(ColorTypeFigure color, int x, int y)
        {
            _typeColor = color;
            _position = new Coords(x, y);
        }

        public void IncrementMoveCount()
        {
            _moveCount++;
        }

    }
}
