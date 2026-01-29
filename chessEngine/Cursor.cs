
namespace chessEngine
{
    public static class Cursor
    {
        public static int CursorX = 0;
        public static int CursorY = 0;
        public static bool Selected = false;
        public static Coords CoordsSeleccionada; 
        public static IFigure FiguraSelected = null;

        public static void FillChess(UI ui)
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if ((x + y) % 2 == 0)
                        ui[x, y] = new Cell("   ", ConsoleColor.Black);
                    else
                        ui[x, y] = new Cell("   ", ConsoleColor.White);
                }
            }
        }
        public static void FillFigures(UI ui, ChessBoard board)
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var fig = board.GetFigureAt(x, y);
                    if (fig == null) continue;
                    string symbol = GetUnicodeFigureChar(fig);
                    var bg = ui[x, y].BackgroundColor;

                    ui[x, y] = new Cell(symbol, bg);
                }
            }
        }
        public static void PaintCursor(UI ui)
        {
            var cell = ui[CursorX, CursorY];
            ui[CursorX, CursorY] = new Cell(cell.Character, ConsoleColor.Yellow);
        }


        public static string GetUnicodeFigureChar(IFigure figure)
        {
            if (figure is King) return figure.ColorType == ColorTypeFigure.BLACK ? " ♔ " : " ♚ ";
            if (figure is Queen) return figure.ColorType == ColorTypeFigure.BLACK ? " ♕ ": " ♛ ";
            if (figure is Rook) return figure.ColorType == ColorTypeFigure.BLACK ? " ♖ " : " ♜ ";
            if (figure is Bishop) return figure.ColorType == ColorTypeFigure.BLACK ? " ♗ " : " ♝ ";
            if (figure is Knight) return figure.ColorType == ColorTypeFigure.BLACK ? " ♘ " : " ♞ ";
            if (figure is Pawn) return figure.ColorType == ColorTypeFigure.BLACK ? " ♙ " : " ♟ ";
            return "?";
        }
        public static void FillAvailablePositions(UI ui, List<Coords> list)
        {
            if (ui == null)
                throw new ArgumentNullException(nameof(ui));
            if (Selected)
            {
                if (list == null || list.Count == 0)
                {
                    Console.WriteLine("La figura seleccionada no tiene movimientos");
                    return;
                }

                foreach (var coord in list)
                {
                    var cell = ui[coord.X, coord.Y];
                    cell.BackgroundColor = ConsoleColor.Green;
                    ui[coord.X, coord.Y] = cell;
                }

            }
        }
        public static void TrySelect(ChessBoard board)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board));

            var fig = board.GetFigureAt(CursorX, CursorY);
            if (fig != null && fig.ColorType == board.CurrentTurn)
            {
                FiguraSelected = fig;
                CoordsSeleccionada = new Coords(CursorX, CursorY);
                Selected = true;
            }
        }
        public static void TryMove(ChessBoard board)
        {
            if(board == null)
                throw new ArgumentNullException(nameof(board));

            if (!Selected || FiguraSelected == null)
                return;

            var destino = new Coords(CursorX, CursorY);
            if (FiguraSelected.IsMovimentValid(destino.X, destino.Y, board))
            {
                board.MoveFigure(CoordsSeleccionada, destino);
            }

            Selected = false;
            FiguraSelected = null;
        }


        public static void ProcesarEntrada(UI ui, ChessBoard board)
        {
            if(ui == null || board == null)
                throw new ArgumentNullException(nameof(ui));
            if (!Console.KeyAvailable)
                return;

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.Spacebar:
                        TrySelect(board);
                    break;
                case ConsoleKey.Enter:
                    TryMove(board);
                    break;

                case ConsoleKey.LeftArrow:
                    if (CursorX > 0)
                        CursorX--;
                    break;
                case ConsoleKey.RightArrow:
                    if (CursorX < 7)
                        CursorX++;
                    break;
                case ConsoleKey.UpArrow:
                    if (CursorY > 0)
                        CursorY--;
                    break;
                case ConsoleKey.DownArrow:
                    if (CursorY < 7)
                        CursorY++;
                    break;
            }
        }

    }
}
