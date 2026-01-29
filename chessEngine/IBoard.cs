namespace chessEngine
{
    public interface IChessBoard
    {
        IFigure GetFigureAt(int x, int y);
        int GetFigureCount();
        ColorTypeFigure SwitchTurn();
        //obtener el ganador haber como lo hago
        //saber si una partida a terminado o no 
        bool IsInside(int x, int y);
        bool IsEmptyFigure(int x, int y);
        bool CanCastleQueenSide(King king);
        bool CanCastleKingSide(King king);



    }
    public interface IBoard : IChessBoard
    {
        void InitBoard();
        void MoveFigure(Coords actual, Coords destino);
        bool IsValidMoveFigure(IFigure figure, int newX, int newY);
        int Height { get; }
        int Width { get; }
        bool IsInCheck2(ColorTypeFigure ColorKing);
        void Clear();
        void SetFigures(List<FigureInit> figures);

    }
}
