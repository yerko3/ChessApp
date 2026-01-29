using System;
using System.Collections.Generic;

namespace chessEngine
{
    public class Pawn : Figure
    {
        public Pawn(ColorTypeFigure color, int x, int y) : base(color, x, y)
        {
        }

        public override List<Coords> GetAvailablePositions(IChessBoard board)
        {
            if (board == null)
                return new List<Coords>();

            List<Coords> result = new List<Coords>();

            result.AddRange(GetBasicPawnMove(board));
            result.AddRange(GetPawnCaptureMoves(board));
            if (IsInitialPositionPeon())
            {
                result.AddRange(GetPawnInitialDoubleStep(board));
            }

            if (board is ChessBoard cb)
            {
                if (!cb.IsInCheck2(ColorType))
                    return result;

                var king = cb.GetFigure(FigureType.KING, ColorType);
                if (king == null)
                    return new List<Coords>();

                if (cb.CanBlockWithAllies(result) || cb.AlliesCanCaptureThreat(result))
                    return result;

                return new List<Coords>(); // no puede salvar al rey
            }

            return result;
        }


        public bool IsPromotionPosition()
        {
            return (this.ColorType == ColorTypeFigure.WHITE && this.Position.Y == 0) ||
                   (this.ColorType == ColorTypeFigure.BLACK && this.Position.Y == 7);
        }

        private bool IsInitialPositionPeon()
        {
            return (this.ColorType == ColorTypeFigure.BLACK && this.Position.Y == 1) ||
                   (this.ColorType == ColorTypeFigure.WHITE && this.Position.Y == 6);
        }

        public override List<Coords> GetThreateningPositions(IChessBoard board)
        {
            if (board == null)
                return new List<Coords>();

            List<Coords> result = new List<Coords>();

            result.AddRange(GetBasicPawnMove(board));
            result.AddRange(GetPawnCaptureMoves(board));
            if (IsInitialPositionPeon())
            {
                result.AddRange(GetPawnInitialDoubleStep(board));
            }
            return result;
        }

        private List<Coords> GetBasicPawnMove(IChessBoard board)//pruvado 
        {
            var result = FigureMovementUtils.GetBasicPawnMove(board, this);
            return result;
        }
        public List<Coords> GetPawnCaptureMoves(IChessBoard board)//private
        {
            var result = FigureMovementUtils.GetPawnCaptureMoves(board, this);
            return result;
        }
        private List<Coords> GetPawnInitialDoubleStep(IChessBoard board)
        {
            var result = FigureMovementUtils.GetPawnInitialDoubleStep(board, this);
            return result;
        }
        public override bool IsMovimentValid(int x, int y, IChessBoard board)
        {
            var list = GetThreateningPositions(board);
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
            return FigureType.PAWN;
        }
        public Figure PromoteTo() 
        {
            if (IsPromotionPosition())
            {
                return new Queen(this.ColorType, this.Position.X, this.Position.Y);
            }
            return null;
        }
        public Figure PromotePawnTo(FigureType newType)
        {
            if (newType == FigureType.PAWN || newType == FigureType.KING)
            {
                throw new InvalidOperationException("No se puede promocionar a un peón o rey.");
            }
            if (IsPromotionPosition())
            {
                switch (newType)
                {
                    case FigureType.QUEEN:
                        return new Queen(this.ColorType, this.Position.X, this.Position.Y);
                    case FigureType.ROOK:
                        return new Rook(this.ColorType, this.Position.X, this.Position.Y);
                    case FigureType.BISHOP:
                        return new Bishop(this.ColorType, this.Position.X, this.Position.Y);
                    case FigureType.KNIGHT:
                        return new Knight(this.ColorType, this.Position.X, this.Position.Y);
                }
            }
            return null; 
        }

    }
}
