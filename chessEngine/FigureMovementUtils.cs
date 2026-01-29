namespace chessEngine
{
    public static class FigureMovementUtils
    {
        public static List<Coords> GetKnightMoves(Knight figure, IChessBoard board)
        {
            if (board == null || figure == null)
                return new List<Coords>();

            var result = new List<Coords>();
            int x = figure.Position.X;
            int y = figure.Position.Y;

            int[] offsetX = { -2, -1, 1, 2, 2, 1, -1, -2 };
            int[] offsetY = { 1, 2, 2, 1, -1, -2, -2, -1 };

            for (int i = 0; i < 8; i++)
            {
                int newX = x + offsetX[i];
                int newY = y + offsetY[i];

                if (!board.IsInside(newX, newY))
                    continue;

                var piece = board.GetFigureAt(newX, newY);
                if (piece == null || piece.ColorType != figure.ColorType)
                    result.Add(new Coords(newX, newY));
            }
            return result;
        }
        public static List<Coords> GetKingMoves(King figure, IChessBoard board)
        {
            if (board == null || figure == null)
                return new List<Coords>();
            List<Coords> result = new List<Coords>();
            int currentX = figure.Position.X;
            int currentY = figure.Position.Y;

            for (int y = currentY - 1; y <= currentY + 1; y++)
            {
                for (int x = currentX - 1; x <= currentX + 1; x++)
                {
                    if (!board.IsInside(x, y))
                        continue;

                    if (x == currentX && y == currentY)
                        continue;

                    var piece = board.GetFigureAt(x, y);
                    if (piece == null || piece.ColorType != figure.ColorType)
                        result.Add(new Coords(x, y));

                }
            }
            return result;
        }
        public static List<Coords> GetQueenMoves(Queen figure, IChessBoard board)
        {
            if (board == null || figure == null)
                return new List<Coords>();
            List<Coords> result = new List<Coords>();

            int x = figure.Position.X;
            int y = figure.Position.Y;

            int[] offsetX = { -1, 1, 0, 0, -1, 1, 1, -1 };
            int[] offsetY = { 0, 0, -1, 1, 1, -1, 1, -1 };

            for (int i = 0; i < 8; i++)
            {
                int newX = x;
                int newY = y;

                while (true)
                {
                    newX += offsetX[i];
                    newY += offsetY[i];

                    if (!board.IsInside(newX, newY))
                        break;

                    if (board.IsEmptyFigure(newX, newY))
                    {
                        result.Add(new Coords(newX, newY));
                    }
                    else
                    {
                        var pieze = board.GetFigureAt(newX, newY);
                        if (pieze != null && pieze.ColorType != figure.ColorType)
                        {
                            result.Add(new Coords(newX, newY));
                        }
                        break;
                    }
                }
            }
            return result;
        }
        public static List<Coords> GetRookMoves(Rook figure, IChessBoard board)
        {
            if (figure == null || board == null)
                return new List<Coords>();
            List<Coords> result = new List<Coords>();

            int x = figure.Position.X;
            int y = figure.Position.Y;

            int[] offsetX = { -1, 1, 0, 0 };
            int[] offsetY = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int newX = x;
                int newY = y;

                while (true)
                {
                    newX += offsetX[i];
                    newY += offsetY[i];

                    if (!board.IsInside(newX, newY))
                        break;

                    if (board.IsEmptyFigure(newX, newY))
                    {
                        result.Add(new Coords(newX, newY));
                    }
                    else
                    {
                        var piece = board.GetFigureAt(newX, newY);
                        if (piece.ColorType != figure.ColorType)
                        {
                            result.Add(new Coords(newX, newY));
                        }
                        break; 
                    }
                }
            }
            return result;
        }
        public static List<Coords> GetBishopMoves(Bishop figure, IChessBoard board)
        {
            if (figure == null || board == null)
                return new List<Coords>();
            List<Coords> result = new List<Coords>();
            int x = figure.Position.X;
            int y = figure.Position.Y;
            int[] offsetX = { -1, 1, 1, -1 };
            int[] offsetY = { -1, -1, 1, 1 };
            for (int i = 0; i < 4; i++)
            {
                int newX = x;
                int newY = y;
                while (true)
                {
                    newX += offsetX[i];
                    newY += offsetY[i];
                    if (!board.IsInside(newX, newY))
                        break;
                    if (board.IsEmptyFigure(newX, newY))
                    {
                        result.Add(new Coords(newX, newY));
                    }
                    else
                    {
                        var piece = board.GetFigureAt(newX, newY);
                        if (piece.ColorType != figure.ColorType)
                        {
                            result.Add(new Coords(newX, newY));
                        }
                        break; 
                    }
                }
            }
            return result;
        }
        public static List<Coords> GetBasicPawnMove(IChessBoard board, Pawn pawn)
        {
            if (board == null || pawn == null)
                return new List<Coords>();

            List<Coords> result = new List<Coords>();

            int x = pawn.Position.X;
            int y = pawn.Position.Y;


            int direction = pawn.ColorType == ColorTypeFigure.BLACK ? 1 : -1;

            int newX = x;
            int newY = y + direction;

            if (board.IsInside(newX, newY) && board.IsEmptyFigure(newX, newY))
            {
                result.Add(new Coords(newX, newY));
            }
            return result;
        }
        public static List<Coords> GetPawnCaptureMoves(IChessBoard board, Pawn pawn)
        {
            if (board == null || pawn == null)
                return new List<Coords>();
            List<Coords> result = new List<Coords>();

            int x = pawn.Position.X;
            int y = pawn.Position.Y;

            int direction = pawn.ColorType == ColorTypeFigure.BLACK ? 1 : -1;

            int[] captureX = { -1, 1 };

            foreach (var offsetX in captureX)
            {
                int newX = x + offsetX;
                int newY = y + direction;

                if (board.IsInside(newX, newY))
                {
                    var piece = board.GetFigureAt(newX, newY);
                    if (piece != null && piece.ColorType != pawn.ColorType)
                    {
                        result.Add(new Coords(newX, newY));
                    }
                }
            }
            return result;
        }
        public static List<Coords> GetPawnInitialDoubleStep(IChessBoard board, Pawn pawn)
        {
            if (board == null || pawn == null)
                return new List<Coords>();

            List<Coords> result = new List<Coords>();


            int x = pawn.Position.X;
            int y = pawn.Position.Y;

            int direction = pawn.ColorType == ColorTypeFigure.BLACK ? 1 : -1;

            int firstStepY = y + direction;
            int secondStepY = y + (2 * direction);

            if (board.IsInside(x, firstStepY) && board.IsEmptyFigure(x, firstStepY))
            {
                result.Add(new Coords(x, firstStepY));
                if (board.IsInside(x, secondStepY) && board.IsEmptyFigure(x, secondStepY))
                {
                    result.Add(new Coords(x, secondStepY));
                }
            }
            return result;
        }
    }
}
