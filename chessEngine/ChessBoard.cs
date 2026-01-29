namespace chessEngine
{
    public delegate void VisitDelegate(Casilla casilla);
    public class ChessBoard : IBoard
    {
        const int _height = 8, _width = 8;
        private readonly Casilla[,] _casillas;
        private ColorTypeFigure _currentTurn;
        private List<IFigure> _capturedFigures = new List<IFigure>();
        public int CaptureFigureCount => _capturedFigures.Count;
        public ColorTypeFigure CurrentTurn
        {
            get => _currentTurn;
            set => _currentTurn = value;
        }


        public ChessBoard()
        {
            _currentTurn = ColorTypeFigure.WHITE;
            _casillas = new Casilla[_height, _width];
            InitBoard();
        }

        public int Height => _height;

        public int Width => _width;

        public void Clear()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _casillas[x, y] = new Casilla
                    {
                        ColorSquare = Utils.GenerateTerritorioType(x + y),
                        Coords = new Coords(x, y)
                    };
                    _casillas[x, y].LiberarCasilla();
                }
            }
        }

        public IFigure GetFigureAt(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                return null;
            return _casillas[x, y].Figure;
        }
        public IFigure GetFigureAt(Coords coords)
        {
            return GetFigureAt(coords.X, coords.Y);
        }
        private Casilla GetCasillaAt(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                throw new Exception("valores fuera del tablero");
            return _casillas[x, y];
        }
        public int GetCasillaCount()
        {
            return _casillas.Length;
        }
        public void SetTurn(ColorTypeFigure turn)
        {
            CurrentTurn = turn;
        }

        public void InitBoard()
        {

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _casillas[x, y] = new Casilla
                    {
                        ColorSquare = Utils.GenerateTerritorioType(x + y),
                        Coords = new Coords(x, y)
                    };
                    _casillas[x, y].OcuparCasilla(Utils.GetDefaultPosition(x, y));
                }
            }
        }

        public ColorTypeSquare GetSquareColor(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                return ColorTypeSquare.NONE;
            var aux = _casillas[x, y];
            return aux == null ? ColorTypeSquare.NONE : _casillas[x, y].ColorSquare;
        }
        public void SetSquareColor(int x, int y, ColorTypeSquare color)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                return;
            if (_casillas[x, y] == null)
                return;
            _casillas[x, y].ColorSquare = color;
        }
        public static ColorTypeSquare SquareColor(int x, int y)
        {
            return (x + y) % 2 == 0 ? ColorTypeSquare.BLUE : ColorTypeSquare.WHITE;
        }
        public bool IsInside(int x, int y)
        {
            return !(x < 0 || x >= _width || y < 0 || y >= _height);
        }
        public bool IsEmptyFigure(Coords coords)
        {
            return IsEmptyFigure(coords.X, coords.Y);
        }
        public bool IsEmptyFigure(int x, int y)
        {
            return _casillas[x, y].Figure == null;
        }

        public int GetFigureCount()
        {
            int count = 0;
            Visit(casilla =>
            {
                if (casilla.Figure != null)
                    count++;
            });
            return count;
        }
        public void Visit(VisitDelegate visit)
        {
            if (visit == null)
                return;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    visit(_casillas[x, y]);
                }
            }
        }
        public ColorTypeFigure SwitchTurn()
        {
            CurrentTurn = CurrentTurn == ColorTypeFigure.WHITE
                          ? ColorTypeFigure.BLACK
                          : ColorTypeFigure.WHITE;
            return CurrentTurn;
        }

        public bool IsValidMoveFigure(IFigure figure, int newX, int newY)
        {
            return figure == null ? false : figure.IsMovimentValid(newX, newY, this);
        }

        public void MoveFigure(Coords actual, Coords destino)
        {
            if (!IsInside(actual.X, actual.Y) || !IsInside(destino.X, destino.Y))
                return;

            var casilla = _casillas[actual.X, actual.Y];
            var destinoCasilla = _casillas[destino.X, destino.Y];

            if (casilla.Figure == null)
                return;

            var figura = casilla.Figure;

            if (figura.ColorType != CurrentTurn)
                return;

            if (!IsValidMoveFigure(figura, destino.X, destino.Y))
                return;

            if (figura is King k && k.MoveCount == 0)
            {
                if ((destino.X == 6 && CanCastleKingSide(k)) ||
                    (destino.X == 2 && CanCastleQueenSide(k)))
                {
                    EjecutarEnroque(k, destino);
                    return;
                }
            }

            var figuraOriginal = destinoCasilla.Figure;
            var origen = figura.Position;

            destinoCasilla.OcuparCasilla(figura);
            casilla.LiberarCasilla();

            if (IsInCheck2(figura.ColorType))
            {
                destinoCasilla.LiberarCasilla();
                casilla.OcuparCasilla(figura);

                if (figuraOriginal != null)
                    destinoCasilla.OcuparCasilla(figuraOriginal);

                return;
            }

            if (figuraOriginal != null)
            {
                _capturedFigures.Add(figuraOriginal);
                Utils.PrintCaptureFigure(_capturedFigures);
            }
            figura.IncrementMoveCount();
            HandlePromotionIfNeeded(figura, destinoCasilla, destino);
            SwitchTurn();
        }


        private void HandlePromotionIfNeeded(IFigure figura, Casilla destinoCasilla, Coords destino)
        {
            if (figura is Pawn p && p.IsPromotionPosition())
            {
                Console.WriteLine($"PEÓN promocionado color: {p.ColorType}, ({p.Position.X},{p.Position.Y})");

                Figure nuevaFigura = p.PromoteTo(); // Usa método de selección del jugador
                if (nuevaFigura != null)
                {
                    destinoCasilla.OcuparCasilla(nuevaFigura);
                    nuevaFigura.Position = destino;
                    Console.WriteLine("¡Peón promocionado!");
                }
            }
        }

        public bool AreBothKingsAlive()
        {
            var WhiteKing = GetFigure(FigureType.KING, ColorTypeFigure.WHITE);
            var BlackKing = GetFigure(FigureType.KING, ColorTypeFigure.BLACK);

            var result = WhiteKing != null && BlackKing != null;
            return WhiteKing != null && BlackKing != null;
        }
        private bool HasNoLegalMoves(ColorTypeFigure color)
        {
            bool anyMoves = false;

            Visit(casilla =>
            {
                var figure = casilla.Figure;
                if (figure != null && figure.ColorType == color)
                {
                    var moves = figure.GetAvailablePositions(this);
                    if (moves != null && moves.Count > 0)
                    {
                        anyMoves = true;
                        return; // salir del delegado
                    }
                }
            });
            return !anyMoves;
        }

        public bool IsCheckmate()
        {
            var king = GetFigure(FigureType.KING, CurrentTurn);
            if (HasNoLegalMoves(CurrentTurn))
                return true;
            if (!AreBothKingsAlive())
                return true;
            if (IsInCheck2(CurrentTurn)) // El rey está en jaque
            {
                if ((king is King k && CanKingEscape(k) && !CanBlockCheck() && !CanCaptureThreat()))
                {
                    Console.WriteLine($"puedo escapara {CanKingEscape(k)}, puedo bloquear {!CanBlockCheck()}," +
                        $"puedo comer la figura {!CanCaptureThreat()}");
                    return true;  // Jaque mate
                }
            }
            return false; // No es jaque mate
        }

        public bool IsInCheck2(ColorTypeFigure ColorKing)
        {
            bool isThreatened = false;
            var king = GetFigure(FigureType.KING, ColorKing);

            if (king == null)
                return false; // Si no hay rey, no hay jaque

            Visit(casilla =>
            {
                var enemyFigure = casilla.Figure as Figure;
                if (enemyFigure != null && enemyFigure.ColorType != king.ColorType)
                {
                    var positionList = enemyFigure.GetThreateningPositions(this);

                    for (int i = 0; i < positionList.Count; i++)
                    {
                        var position = king.Position;
                        if (position.X == positionList[i].X && position.Y == positionList[i].Y)
                        {
                            isThreatened = true;
                            return; 
                        }
                    }
                }
            });

            return isThreatened;
        }

        public bool CanKingEscape(King figureKing) 
        {
            return figureKing != null && GetSafeKingMoves(figureKing).Count > 0;
        }
        public List<Coords> GetSafeKingMoves(King figureKing)
        {
            if (figureKing == null)
                throw new Exception("Figura nula");

            List<Coords> safePositions = new List<Coords>();
            var kingPositions = figureKing.GetThreateningPositions(this); 

            for (int i = 0; i < kingPositions.Count; i++)
            {
                if (!IsPositionUnderThreat(figureKing, kingPositions[i])) 
                    safePositions.Add(kingPositions[i]);
            }
            return safePositions;
        }
        private bool CanCaptureThreat()
        {
            var king = GetFigure(FigureType.KING, CurrentTurn);
            if (king == null)
                return false;

            var threats = GetThreatSources(king, king.Position); 

            if (threats.Count == 0)
                return false;

            bool canCapture = false;

            Visit(casilla =>
            {
                var aliados = casilla.Figure;
                if (aliados != null && aliados.ColorType == king.ColorType && aliados != king)
                {
                    foreach (var threatPos in threats)
                    {
                        if (aliados.IsMovimentValid(threatPos.X, threatPos.Y, this))
                        {
                            canCapture = true;
                            return; 
                        }
                    }
                }
            });

            return canCapture;
        }

        public bool CanBlockCheck()
        {
            return GetBlockingPositions().Count > 0;
        }

        private List<Coords> GetBlockingPositions()
        {
            var king = GetFigure(FigureType.KING,CurrentTurn);
            List<Coords> result = new List<Coords>();
            var attackablePositions = GetAttackingPositions();


            Visit(casilla =>
            {
                var FriendFigure = casilla.Figure;
                if (FriendFigure != null && FriendFigure.ColorType == king.ColorType && FriendFigure != king && FriendFigure is Figure f)
                {
                    var listAvailablePositions = f.GetThreateningPositions(this);

                    for (int i = 0; i < attackablePositions.Count; i++)
                    {
                        for (int j = 0; j < listAvailablePositions.Count; j++)
                        {
                            if (attackablePositions[i].X == listAvailablePositions[j].X && attackablePositions[i].Y == listAvailablePositions[j].Y)
                            {
                                result.Add(new Coords(listAvailablePositions[j].X, listAvailablePositions[j].Y));
                            }
                        }
                    }
                }
            });
            return result;
        }

        private List<Coords> GetAttackingPositions()
        {
            var result = new List<Coords>();
            var king = GetFigure(FigureType.KING, CurrentTurn);
            if (king == null)
                return result;

            var listaEnemyFigures = GetThreatSources(king, king.Position);

            if (listaEnemyFigures.Count != 1) 
                return result; 

            var enemyPos = listaEnemyFigures[0];
            var enemyFigure = GetFigureAt(enemyPos);

            if (enemyFigure == null || enemyFigure.ColorType == king.ColorType)
                return result;

            if (enemyFigure is Queen q)
                result = q.GetThreateningPositions(king.Position, this);
            else if (enemyFigure is Bishop b)
                result = b.GetThreateningPositions(king.Position, this);
            else if (enemyFigure is Rook r)
                result = r.GetThreateningPositions(king.Position, this);

            return result;
        }


        public bool IsPositionUnderThreat(Figure targetFigure, Coords position)
        {
            if (targetFigure == null)
                return false;
            bool isThreatened = false;
            Visit(casilla =>
            {

                var enemyFigure = casilla.Figure;
                if (enemyFigure != null && enemyFigure.ColorType != targetFigure.ColorType && enemyFigure is Figure f)
                {
                    var attackablePositions = f.GetThreateningPositions(this);

                    foreach (var attackPos in attackablePositions)
                    {
                        if (attackPos.X == position.X && attackPos.Y == position.Y)
                        {
                            isThreatened = true;
                            return; 
                        }
                    }
                }
            });
            return isThreatened;
        }
        private List<Coords> GetThreatSources(IFigure target, Coords position)
        {
            if (target == null)
                return new List<Coords>();
            List<Coords> threatSources = new List<Coords>();

            Visit(casilla =>
            {
                var enemyFigure = casilla.Figure;
                if (enemyFigure != null && enemyFigure.ColorType != target.ColorType && enemyFigure is Figure f)
                {
                    var attackablePositions = f.GetThreateningPositions(this);
                    foreach (var attackPos in attackablePositions)
                    {
                        if (attackPos.X == position.X && attackPos.Y == position.Y)
                        {
                            threatSources.Add(enemyFigure.Position);
                            break;
                        }
                    }
                }
            });
            return threatSources;
        }

        public IFigure GetFigure(FigureType figuretype)
        {
            IFigure temp = null;
            Visit(casilla =>
            {
                var figure = casilla.Figure;
                if (figure == null)
                    return;
                if (figure.GetFigureType() == figuretype)
                    temp = casilla.Figure;
            });
            return temp;
        }
        public IFigure GetFigure(FigureType figuretype, ColorTypeFigure colorType)
        {
            IFigure temp = null;
            Visit(casilla =>
            {
                var figure = casilla.Figure;
                if (figure == null)
                    return;
                if (figure.GetFigureType() == figuretype && figure.ColorType == colorType)
                    temp = casilla.Figure;
            });
            return temp;
        }
        public bool CanCastleKingSide(King king)
        {
            if (king == null || king.HasMove)
                return false;

            var isWhite = king.ColorType == ColorTypeFigure.WHITE;
            var y = isWhite ? 7 : 0;

            if (king.Position.X != 4 || king.Position.Y != y)
                return false;

            var rookCasilla = GetCasillaAt(7, y);
            if (!(rookCasilla.Figure is Rook rook) || rook.ColorType != king.ColorType || rook.HasMove)
                return false;

            if (!IsEmptyFigure(5, y) || !IsEmptyFigure(6, y))
                return false;

            var positionsToCheck = new[]
            {
                new Coords(4, y), // donde está el rey
                new Coords(5, y),
                new Coords(6, y)
            };

            foreach (var pos in positionsToCheck)
            {
                if (IsPositionUnderThreat(king, pos))
                    return false;
            }

            return true;
        }

        private void EjecutarEnroque(King king, Coords destino)
        {
            if (king == null)
                return;
            int y = king.ColorType == ColorTypeFigure.WHITE ? 7 : 0;
            if (destino.X == 6 && CanCastleKingSide(king))
            {
                var torre = GetCasillaAt(7, y).Figure;
                GetCasillaAt(7, y).LiberarCasilla();
                GetCasillaAt(5, y).OcuparCasilla(torre);
                GetCasillaAt(6, y).OcuparCasilla(king);
            }
            else if (destino.X == 2 && CanCastleQueenSide(king))
            {
                var torre = GetCasillaAt(0, y).Figure;
                GetCasillaAt(0, y).LiberarCasilla();
                GetCasillaAt(3, y).OcuparCasilla(torre);
                GetCasillaAt(2, y).OcuparCasilla(king);
            }
            GetCasillaAt(4, y).LiberarCasilla();
            king.Position = destino; 

        }

        public bool CanCastleQueenSide(King king)
        {
            if (king == null || king.HasMove)
                return false;

            var isWhite = king.ColorType == ColorTypeFigure.WHITE;
            var y = isWhite ? 7 : 0;

            if (king.Position.X != 4 || king.Position.Y != y)
                return false;

            var rookCasilla = GetCasillaAt(0, y);
            if (!(rookCasilla.Figure is Rook rook) || rook.ColorType != king.ColorType || rook.HasMove)
                return false;

            if (!IsEmptyFigure(1, y) || !IsEmptyFigure(2, y) || !IsEmptyFigure(3, y))
                return false;

            var positionsToCheck = new[]
            {
             new Coords(4, y), 
             new Coords(3, y), 
             new Coords(2, y) 
            };

            foreach (var pos in positionsToCheck)
            {
                if (IsPositionUnderThreat(king, pos))
                    return false;
            }

            return true;
        }

        public bool AlliesCanCaptureThreat(List<Coords> result)
        {
            var king = GetFigure(FigureType.KING, CurrentTurn);
            if (king == null)
                return false;

            var threats = GetThreatSources(king, king.Position);
            if (threats.Count == 0)
                return false;

            foreach (var move in result)
            {
                foreach (var threat in threats)
                {
                    if (move.X == threat.X && move.Y == threat.Y)
                        return true;
                }
            }
            return false;
        }

        public bool CanBlockWithAllies(List<Coords> result)
        {
            var blockingPositions = GetBlockingPositions();
            if (blockingPositions.Count == 0)
                return false;

            foreach (var move in result)
            {
                foreach (var block in blockingPositions)
                {
                    if (move.X == block.X && move.Y == block.Y)
                        return true;
                }
            }

            return false;
        }
        public List<FigureInit> GetFiguresDescription()
        {
            List<FigureInit> list = new();
            Visit(casilla =>
            {
                if (casilla.Figure is Figure fig)
                {
                    list.Add(new FigureInit(fig.GetFigureType(),fig.ColorType,fig.Position));
                }
            });
            return list;
        }

        public void SetFigures(List<FigureInit> figures)
        {
            if(figures == null || figures.Count == 0)
                return;
            Clear();
            foreach (var desc in figures)
            {
                var figure = FigureFactory.CreateFigure(desc.Type, desc.Color, desc.Position);
                GetCasillaAt(desc.Position.X, desc.Position.Y).OcuparCasilla(figure);
            }
        }
    }
}
