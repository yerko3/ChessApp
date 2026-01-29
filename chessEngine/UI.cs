using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Xml;

namespace chessEngine
{
    public struct Cell
    {
        public string Character;
        public ConsoleColor BackgroundColor;

        public Cell(string character, ConsoleColor backgroundColor)
        {
            Character = character;
            BackgroundColor = backgroundColor;
        }
    }
    public class UI
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Cell[,] _buffer;

        public int Width => _width;
        public int Height => _height;
        public Cell this[int x, int y]
        {
            get => GetCellAt(x, y);
            set => SetCellAt(x, y, value);
        }
        public UI(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");

            _width = width;
            _height = height;
            _buffer = new Cell[height, width];
        }
        public void Clear()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _buffer[y, x].Character = "  ";
                    _buffer[y, x].BackgroundColor = ConsoleColor.Black;
                }
            }
        }

        public void Show()
        {
            var oldBackgroundColor = Console.BackgroundColor;
            var oldForegroundColor = Console.ForegroundColor;

            for (int y = 0; y < _height; y++)
            {
                Console.SetCursorPosition(0, y);
                for (int x = 0; x < _width; x++)
                {
                    Console.BackgroundColor = _buffer[y, x].BackgroundColor;
                    Console.Write(_buffer[y, x].Character);
                }
            }
            Console.BackgroundColor = oldBackgroundColor;
            Console.ForegroundColor = oldForegroundColor;
            Console.SetCursorPosition(0, _height);
        }

        public Cell GetCellAt(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                throw new ArgumentOutOfRangeException("Invalid coordinates");
            return _buffer[y, x];
        }

        private void SetCellAt(int x, int y, Cell value)
        {
            if (x < 0 || x >= _width)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= _height)
                throw new ArgumentOutOfRangeException("y");

            _buffer[y, x] = value;
        }

        public void PrintString(int x, int y, string text, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (y < 0 || y >= _height)
                throw new ArgumentOutOfRangeException(nameof(y));
            if (x < 0 || x >= _width)
                throw new ArgumentOutOfRangeException(nameof(x));

            for (int i = 0; i < text.Length && (x + i) < _width; i++)
            {
                Cell c = new Cell()
                {
                    Character = " " + text[i],
                    BackgroundColor = backgroundColor,
                };

                this[x + i, y] = c;
            }
        }

        private static int _movimientos = 1;
        private static void ExecuteMove(ChessBoard board)
        {
            if (board == null)
                return;
            Console.WriteLine("        ____ CHESS ____");
            Console.WriteLine("Recuerda que la x de este tablero esta representado con A,B,C ...");
            Console.WriteLine("Y la y esta representado con 0,1,2,3...");
            Console.WriteLine("    A    B   C   D   E   F   G   H");
            Utils.PrintBoard(board);
        inicio:
            Console.WriteLine("Dame la x e y de la pieza que quires coger");
            (int x, int y) = GetValidCoords(Console.ReadLine());
            if (!IsInsideCoords(board, x, y))
            {
                Console.WriteLine("Coordenas no validas fuera del tablero, vuelvo a intentarlo.");
                goto inicio;
            }
            var figure = board.GetFigureAt(x, y);
            if (figure == null)
            {
                Console.WriteLine("La x e y no pertenece a ninguna figura del tablero, vuelva a intentarlo");
                goto inicio;
            }
            var ListMovePosition = figure.GetAvailablePositions(board);
            if (ListMovePosition.Count == 0)
            {
                Console.WriteLine("La pieza que quieres coger no puede hacer movimientos, vuelve a intentarlo");
                goto inicio;
            }
            Coords coordActuales = new Coords(x, y);
            if (_movimientos % 2 != 0 && figure.ColorType == ColorTypeFigure.WHITE)
            {
                Utils.PaintMoveRange(board, ListMovePosition);
                Console.WriteLine("    A    B   C   D   E   F   G   H");
                Utils.PrintBoard(board);
                Utils.PrintResetColorMove(board, ListMovePosition);
            }
            else if (_movimientos % 2 == 0 && figure.ColorType == ColorTypeFigure.BLACK)
            {
                Utils.PaintMoveRange(board, ListMovePosition);
                Console.WriteLine("    A    B   C   D   E   F   G   H");
                Utils.PrintBoard(board);
                Utils.PrintResetColorMove(board, ListMovePosition);
            }
            else
            {
                Console.WriteLine("La pieza del color es incorrecta, vuelve a intentarlo");
                goto inicio;
            }
        Medio:
            Console.WriteLine("Dame la x e y de la pieza que quires ir");
            (int xdes, int ydes) = GetValidCoords(Console.ReadLine());
            if (!IsInsideCoords(board, xdes, ydes))
            {
                Console.WriteLine("Coordenas no validas fuera del tablero, vuelvo a intentarlo.");
                goto Medio;
            }
            Coords coordsDespues = new Coords(xdes, ydes);
            if (!figure.IsMovimentValid(xdes, ydes, board))
            {
                Console.WriteLine("movimiento invalido intentelo de nuevo");
                goto Medio;
            }
            board.MoveFigure(coordActuales, coordsDespues);
            _movimientos++;
            Console.WriteLine("        ____ CHESS ____");
            Console.WriteLine("    A    B   C   D   E   F   G   H");
            Utils.PrintBoard(board);
        }

        private static bool IsNumberStr(char value)
        {
                if (value < '0' || value > '9')
                    return false;
            return true;
        }
        private static bool IsInsideCoords(ChessBoard board, int x, int y)
        {
            if (board == null)
                return false;
            return board.IsInside(x,y);
        }
        public static void ExecuteTurn(ChessBoard board)
        {
            if (board == null)
                return;


            while (true)
            {                
                if (board.IsCheckmate())
                {
                    Console.WriteLine("Game Over! Checkmate!");
                    break;
                }
                if (!board.AreBothKingsAlive())
                {
                    Console.WriteLine("Game Over! Unos de los reyes a caido");
                    break;
                }
                ExecuteMove(board);
            }
        }
        private static int ConverLettersInInt(char letter)
        {
            if (letter < 'A' || letter > 'H')
                return -1;
            return letter - 'A';
        }
        private static int ConverNumberAint(char letter)
        {
            if (letter < '0' || letter > '7')
                return -1;
            return letter - '0';
        }
        private static bool IsLetterStr(string value)
        {
            if (value == null)
                return false;
            for (int i = 0; i < value.Length; i++)
            {
                char caracter = value[i];
                if (caracter < 'A' || caracter > 'H')
                    return false;
            }
            return true;
        }
        private static bool IsLetterStr(char value)
        {
                if (value < 'A' || value > 'H')
                    return false;
            return true;
        }
        private static (int, int) GetValidCoords(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 2)
            {
                Console.WriteLine("Entrada no válida. Intente de nuevo:");
                return GetValidCoords(Console.ReadLine());
            }

            char letter = value[0];
            char number = value[1];

            if (IsLetterStr(letter) && IsNumberStr(number))
            {
                return (ConverLettersInInt(letter), ConverNumberAint(number));
            }

            Console.WriteLine("Entrada no válida. Intente de nuevo:");
            return GetValidCoords(Console.ReadLine());
        }


    }
}
