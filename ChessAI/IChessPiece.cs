using ChessAI.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessAI
{
    public interface IChessPiece
    {

        # region Implemented

        public int Worth { get; }

        public string Texture
        {
            get;
            set;
        }

        public Team Team
        {
            get;
            set;
        }

        public bool Moved
        {
            get;
            set;
        }

        public abstract IChessPiece Copy();

        public bool UnderAttack(in IChessPiece[] chessBoard, ChessPosition chessPosition)
        {
            bool isAttacking = false;

            foreach (IChessPiece chessPiece in chessBoard)
            {
                if (chessPiece != null && chessPiece.CanMove(chessBoard, new ChessMove(chessPosition, chessPosition)))
                    isAttacking = true;
            }

            return isAttacking;
        }

        public static bool UnderAttack(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            bool isAttacking = false;

            IChessPiece[] newChessBoard = new IChessPiece[chessBoard.Length];
            chessBoard.CopyTo(newChessBoard, 0);

            int fromIndex = chessMove.From.ToIndex();
            IChessPiece chessPiece1 = newChessBoard[fromIndex];

            newChessBoard[fromIndex] = null;
            newChessBoard[chessMove.To.ToIndex()] = chessPiece1;

            foreach (IChessPiece chessPiece in newChessBoard)
            {
                if (chessPiece != null && chessPiece.CanMove(newChessBoard, chessMove))
                    isAttacking = true;
            }

            return isAttacking;
        }

        static public bool AttacksKing(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            IChessPiece[] newChessBoard = new IChessPiece[chessBoard.Length];
            chessBoard.CopyTo(newChessBoard, 0);

            int fromIndex = chessMove.From.ToIndex();
            int toIndex = chessMove.To.ToIndex();

            IChessPiece chessPiece1 = newChessBoard[fromIndex];

            newChessBoard[fromIndex] = null;
            newChessBoard[toIndex] = chessPiece1;

            ChessPosition kingPosition = new ChessPosition();

            int index = 0;
            foreach (IChessPiece chessPiece in newChessBoard)
            {
                if (chessPiece != null && chessPiece.GetType() == typeof(King) && chessPiece.Team == chessPiece1.Team)
                {
                    kingPosition = new ChessPosition(index);
                    break;
                }
                index++;
            }

            index = 0;
            foreach (IChessPiece chessPiece in newChessBoard)
            {
                if (chessPiece != null && chessPiece.CanMove(newChessBoard, new ChessMove(new ChessPosition(index), kingPosition)))
                {
                    return true;
                }
                index++;
            }

            return false;
        }

        public List<ChessPosition> GetAttacks(in IChessPiece[] chessBoard, ChessPosition position)
        {
            List<ChessPosition> attacks = new List<ChessPosition>();

            foreach (ChessPosition possibleAttack in GetValidMoves(chessBoard, position))
            {
                int attackIndex = possibleAttack.ToIndex();
                if (chessBoard[attackIndex] != null && !AttacksKing(chessBoard, new ChessMove(position, possibleAttack)) && chessBoard[attackIndex].Team != chessBoard[position.ToIndex()].Team)
                    attacks.Add(possibleAttack);
            }

            return attacks;
        }

        public bool CanMove(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            if (
                (chessMove.To.X < 8 || chessMove.To.X >= 0 || chessMove.To.Y < 8 || chessMove.To.Y >= 0) &&
                (chessMove.To.X != chessMove.From.X || chessMove.To.Y != chessMove.From.Y) &&
                chessMove.From.ToIndex() < 80 && chessMove.From.ToIndex() >= 0 && chessBoard[chessMove.From.ToIndex()] != null
            )
            {
                if (chessBoard[chessMove.To.ToIndex()] != null && chessBoard[chessMove.To.ToIndex()].Team == chessBoard[chessMove.From.ToIndex()].Team)
                {
                    return false;
                }

                return CanMoveInternal(in chessBoard, chessMove);
            }

            return false;
        }

        protected bool CanMoveInternal(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            return true;
        }

        public void Move(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            Moved = true;

            int fromIndex = chessMove.From.ToIndex();
            IChessPiece chessPiece = chessBoard[fromIndex];

            chessBoard[fromIndex] = null;
            chessBoard[chessMove.To.ToIndex()] = chessPiece;
        }

        public List<ChessPosition> GetValidMoves(in IChessPiece[] chessBoard, ChessPosition position)
        {
            return new List<ChessPosition>();
        }

        static internal bool Displace(in IChessPiece[] chessBoard, in List<ChessPosition> moves, ChessMove chessMove)
        {
            if (chessMove.To.X > 7 || chessMove.To.X < 0 || chessMove.To.Y > 7 || chessMove.To.Y < 0)
                return false;

            int toIndex = chessMove.To.ToIndex();

            if (chessBoard[toIndex] != null)
            {
                if (chessBoard[toIndex].Team != chessBoard[chessMove.From.ToIndex()].Team && !AttacksKing(chessBoard, chessMove))
                {
                    moves.Add(chessMove.To);
                }

                return false;
            }

            if (AttacksKing(chessBoard, chessMove))
                return false;

            moves.Add(chessMove.To);
            return true;
        }

        #endregion

        #region Optimized (Interface only)

        public List<ChessPosition> GetValidMovesOptimized(IChessPiece[] chessBoard, ChessPosition position)
        {
            List<ChessPosition> chessPositions = GetValidMoves(chessBoard, position);

            return chessPositions.OrderBy(newPosition => {
                return (int)MathF.Abs(newPosition.ToIndex() - position.ToIndex());
            
            }).ToList();
        }

        #endregion
    }
}