using System;
using System.Collections.Generic;

namespace ChessAI.ChessPieces
{
    public class Pawn : IChessPiece
    {
        public Pawn(Team team, bool forward, bool moved = false, bool movedTwo = false)
        {
            Texture = TextureAlias.GetTexture(Piece.Pawn, team);
            Forward = forward;
            Team = team;
            Moved = moved;
            MovedTwo = movedTwo;
        }

        public int Worth { get => 100; }

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

        public bool MovedTwo
        {
            get;
            set;
        }

        public bool Forward;

        public IChessPiece Copy()
        {
            return new Pawn(Team, Forward, Moved, MovedTwo);
        }

        void IChessPiece.Move(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            if (chessMove.From.X != chessMove.To.X && chessBoard[chessMove.To.ToIndex()] == null)
                chessBoard[new ChessPosition(chessMove.To.X, chessMove.To.Y - (Forward ? -1 : 1)).ToIndex()] = null;

            if (MovedTwo)
                MovedTwo = false;
            if (!Moved)
            {
                if (Math.Abs(chessMove.From.Y - chessMove.To.Y) == 2)
                {
                    MovedTwo = true;
                }
                Moved = true;
            }

            IChessPiece chessPiece = chessBoard[chessMove.From.ToIndex()];

            chessBoard[chessMove.From.ToIndex()] = null;
            if (chessMove.To.ToIndex() == (Forward ? ChessPosition.CreateIndex(chessMove.To.X, 0) : ChessPosition.CreateIndex(chessMove.To.X, 7)))
            {
                chessPiece = new Queen(Team);
            }
            chessBoard[chessMove.To.ToIndex()] = chessPiece;
        }

        private bool CanMove(in IChessPiece[] chessBoard, ChessMove chessMove)
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

        bool IChessPiece.CanMove(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            return CanMove(in chessBoard, chessMove);
        }

        private bool CanMoveInternal(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            if (
                    (chessMove.To.Y == (chessMove.From.Y + (Forward ? -1 : 1)) ||
                    (!Moved && (chessMove.To.Y == (chessMove.From.Y + (Forward ? -2 : 2))) && chessBoard[ChessPosition.CreateIndex(chessMove.To.X, chessMove.From.Y + (Forward ? -1 : 1))] == null)) &&
                    chessMove.To.X == chessMove.From.X && chessBoard[chessMove.To.ToIndex()] == null
            )
                return true;
            else if ((chessMove.To.Y == (chessMove.From.Y + (Forward ? -1 : 1))) && Math.Abs(chessMove.From.X - chessMove.To.X) == 1)
            {
                int checkingPosition = new ChessPosition(chessMove.To.X, chessMove.From.Y).ToIndex();
                if (chessBoard[chessMove.To.ToIndex()] != null && chessBoard[chessMove.To.ToIndex()].Team != Team)
                    return true;
                else if (
                    Math.Abs(chessMove.From.X - chessMove.To.Y) == 1 && chessBoard[checkingPosition] != null &&
                    chessBoard[checkingPosition] is Pawn && ((Pawn)chessBoard[checkingPosition]).MovedTwo && chessBoard[checkingPosition].Team != Team
                )
                    return true;
            }

            return false;
        }

        List<ChessPosition> IChessPiece.GetValidMoves(in IChessPiece[] chessBoard, ChessPosition position)
        {
            List<ChessPosition> validMoves = new List<ChessPosition>();

            ChessPosition newPosition = new ChessPosition(position.X, position.Y + (Forward ? -1 : 1));

            if (
                newPosition.Y < 8 &&
                newPosition.Y >= 0 &&
                chessBoard[newPosition.ToIndex()] == null &&
                !IChessPiece.AttacksKing(chessBoard, new ChessMove(position, newPosition))
            )
                validMoves.Add(newPosition);

            if (!Moved)
            {
                ChessPosition forwardPosition = new ChessPosition(position.X, position.Y + (Forward ? -2 : 2));

                if (
                newPosition.Y < 8 &&
                newPosition.Y >= 0 &&
                forwardPosition.Y < 8 &&
                forwardPosition.Y >= 0 &&
                chessBoard[newPosition.ToIndex()] == null &&
                chessBoard[forwardPosition.ToIndex()] == null &&
                !IChessPiece.AttacksKing(chessBoard, new ChessMove(position, forwardPosition))
            )
                    validMoves.Add(forwardPosition);
            }

            newPosition = new ChessPosition(position.X + 1, position.Y + (Forward ? -1 : 1));
            int newPositionIndex = newPosition.ToIndex();

            if (newPositionIndex < 78 && newPositionIndex >= 0 && chessBoard[newPositionIndex] != null)
                IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, newPosition));

            newPosition.X = position.X - 1;
            newPositionIndex = newPosition.ToIndex();

            if (newPositionIndex < 78 && newPositionIndex >= 0 && chessBoard[newPositionIndex] != null)
                IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, newPosition));

            ChessPosition removingPosition = new ChessPosition(position.X - 1, position.Y);
            int removingPositionIndex = removingPosition.ToIndex();

            if (
                newPositionIndex < 78 && newPositionIndex >= 0 &&
                removingPositionIndex < 78 && removingPositionIndex >= 0 &&
                chessBoard[removingPositionIndex] is Pawn && ((Pawn)chessBoard[removingPositionIndex]).MovedTwo && chessBoard[removingPositionIndex].Team != Team &&
                !IChessPiece.AttacksKing(in chessBoard, new ChessMove(removingPosition, new ChessPosition(position.X - 1, position.Y + 1)))
            )
                IChessPiece.Displace(in chessBoard, validMoves, new ChessMove(position, newPosition));

            newPosition.X = position.X + 2;
            newPositionIndex = newPosition.ToIndex();

            if (
                newPositionIndex < 78 && newPositionIndex >= 0 &&
                removingPositionIndex < 78 && removingPositionIndex >= 0 &&
                chessBoard[removingPositionIndex] is Pawn && ((Pawn)chessBoard[removingPositionIndex]).MovedTwo && chessBoard[removingPositionIndex].Team != Team &&
                !IChessPiece.AttacksKing(in chessBoard, new ChessMove(removingPosition, new ChessPosition(position.X + 1, position.Y + 1)))
            )
                IChessPiece.Displace(in chessBoard, validMoves, new ChessMove(position, newPosition));

            return validMoves;
        }
    }
}