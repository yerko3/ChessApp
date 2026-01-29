namespace chessEngine
{
    public enum ColorTypeSquare
    {
        BLUE, WHITE, DARKGREY, BLACK, YELLOW, NONE
    }
    public class FigureInit
    {
        public FigureType Type { get; set; }
        public ColorTypeFigure Color { get; set; }
        public Coords Position { get; set; }

        public FigureInit() { }
        public FigureInit(FigureType type, ColorTypeFigure color, Coords position)
        {
            Type = type;
            Color = color;
            Position = position;
        }
    }
    public class FigureFactory
    {
        // Esto es una  fábrica de figuras: recibe el tipo de figura y devuelve un objeto real que implementa Figure
        public static Figure CreateFigure(FigureType tipo, ColorTypeFigure color, Coords coords)
        {
            return tipo switch
            {
                FigureType.KING => new King(color, coords.X, coords.Y),
                FigureType.QUEEN => new Queen(color, coords.X, coords.Y),
                FigureType.ROOK => new Rook(color, coords.X, coords.Y),
                FigureType.BISHOP => new Bishop(color, coords.X, coords.Y),
                FigureType.KNIGHT => new Knight(color, coords.X, coords.Y),
                FigureType.PAWN => new Pawn(color, coords.X, coords.Y),
                _ => throw new Exception("Tipo de figura inválido")
            };
        }
    }
    public class Utils
    {
        private static readonly Random _random = new Random();

        private static void PrintFigure(IFigure figure)
        {
            if (figure == null)
            {
                Console.Write("   ");
                return;
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var color = figure.ColorType;

            if (figure is King && color == ColorTypeFigure.BLACK)
            {
                Console.Write("♔  ");
            }
            else if (figure is King && color == ColorTypeFigure.WHITE)
            {
                Console.Write("♚  ");
            }
            else if (figure is Queen && color == ColorTypeFigure.BLACK)
            {
                Console.Write("♕  ");
            }
            else if (figure is Queen && color == ColorTypeFigure.WHITE)
            {
                Console.Write("♛  ");
            }
            else if (figure is Pawn && color == ColorTypeFigure.BLACK)
            {
                Console.Write("♙  ");
            }
            else if (figure is Pawn && color == ColorTypeFigure.WHITE)
            {
                Console.Write("♟  ");
            }
            else if (figure is Rook && color == ColorTypeFigure.BLACK)
            {
                Console.Write("♖  ");
            }
            else if (figure is Rook && color == ColorTypeFigure.WHITE)
            {
                Console.Write("♜  ");
            }
            else if (figure is Bishop && color == ColorTypeFigure.BLACK)
            {
                Console.Write("♗  ");
            }
            else if (figure is Bishop && color == ColorTypeFigure.WHITE)
            {
                Console.Write("♝  ");
            }
            else if (figure is Knight && color == ColorTypeFigure.BLACK)
            {
                Console.Write("♘  ");
            }
            else if (figure is Knight && color == ColorTypeFigure.WHITE)
            {
                Console.Write("♞  ");
            }
        }
        public static ColorTypeSquare GenerateTerritorioType(int index)
        {
            if (index % 2 == 0)
                return ColorTypeSquare.BLUE;
            return ColorTypeSquare.WHITE;
        }

        private static void PrintSquare(ColorTypeSquare square)
        {
            if (square == ColorTypeSquare.BLUE)
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write(" ");
            }
            if (square == ColorTypeSquare.WHITE)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write(" ");
            }
            if (square == ColorTypeSquare.DARKGREY)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.Write(" ");
            }
            if (square == ColorTypeSquare.BLACK)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(" ");
            }
            if (square == ColorTypeSquare.YELLOW)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write(" ");
            }
        }
        public static void PrintBoard(ChessBoard board)
        {
            int count = 0;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var square = board.GetSquareColor(x, y);
                    var Piece = board.GetFigureAt(x, y); //tener cuidado no estoy verficando que piece sea nulo ya que si lo hago PrintFigure no pudo poner un cw 
                                                         //para que cuadre el tablero 

                    if (square != ColorTypeSquare.NONE)
                    {
                        if (x == 0)
                            Console.Write(" " + count++ + " ");
                        PrintSquare(square);
                        PrintFigure((Figure)Piece);

                    }
                    else
                        Console.Write(" ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        public static void PrintResetColorMove(ChessBoard board, int x, int y)
        {
            if (board == null)
                return;
            var square = board.GetSquareColor(x, y);
            var aux = x + y;
            if (aux % 2 == 0)
                board.SetSquareColor(x, y, ColorTypeSquare.BLUE);
            else
                board.SetSquareColor(x, y, ColorTypeSquare.WHITE);
        }
        public static void PrintResetColorMove(ChessBoard board, List<Coords> list)
        {
            if (board == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                var square = board.GetSquareColor(list[i].X, list[i].Y);
                var aux = list[i].X + list[i].Y;
                if (aux % 2 == 0)
                    board.SetSquareColor(list[i].X, list[i].Y, ColorTypeSquare.BLUE);
                else
                    board.SetSquareColor(list[i].X, list[i].Y, ColorTypeSquare.WHITE);
            }
        }
        public static void PrintCaptureFigure(List<IFigure> capturedPieces)
        {
            if (capturedPieces.Count == 0)
                return;
            for (int i = 0; i < capturedPieces.Count; i++)
            {
                if (capturedPieces[i].ColorType == ColorTypeFigure.WHITE)
                {
                    PrintFigure(capturedPieces[i]);
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
            foreach (var piece in capturedPieces)
            {
                if (piece.ColorType == ColorTypeFigure.BLACK)
                {
                    PrintFigure(piece);  // Imprimir figura negra
                    Console.Write(" ");
                }
            }
            Console.WriteLine();

        }
        public static void PaintMoveRange(ChessBoard board, List<Coords> list)
        {
            if (board == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                var square = board.GetSquareColor(list[i].X, list[i].Y);
                var aux = list[i].X + list[i].Y;
                if (aux % 2 == 0)
                    board.SetSquareColor(list[i].X, list[i].Y, ColorTypeSquare.DARKGREY);
                else
                    board.SetSquareColor(list[i].X, list[i].Y, ColorTypeSquare.BLACK);
            }
        }
        public static void PrintLettersAndClear()
        {
            Console.Clear();
            Console.WriteLine("        ____ CHESS ____");
            Console.WriteLine("    A    B   C   D   E   F   G   H");
        }
        public static Figure GetDefaultPosition(int x, int y)
        {

            if (x < 0 || x > 7 || y < 0 || y > 7)
                return null;

            if (y == 0)
            {
                if (x == 0 || x == 7)
                    return new Rook(ColorTypeFigure.BLACK, x, y);
                if (x == 1 || x == 6)
                    return new Knight(ColorTypeFigure.BLACK, x, y);
                if (x == 2 || x == 5)
                    return new Bishop(ColorTypeFigure.BLACK, x, y);
                if (x == 3)
                    return new Queen(ColorTypeFigure.BLACK, x, y);
                if (x == 4)
                    return new King(ColorTypeFigure.BLACK, x, y);
            }
            else if (y == 7)
            {
                if (x == 0 || x == 7)
                    return new Rook(ColorTypeFigure.WHITE, x, y);
                if (x == 1 || x == 6)
                    return new Knight(ColorTypeFigure.WHITE, x, y);
                if (x == 2 || x == 5)
                    return new Bishop(ColorTypeFigure.WHITE, x, y);
                if (x == 3)
                    return new Queen(ColorTypeFigure.WHITE, x, y);
                if (x == 4)
                    return new King(ColorTypeFigure.WHITE, x, y);
            }
            else if (y == 1)
            {
                return new Pawn(ColorTypeFigure.BLACK, x, y);
            }
            else if (y == 6)
            {
                return new Pawn(ColorTypeFigure.WHITE, x, y);
            }
            return null;
        }


    }
}
