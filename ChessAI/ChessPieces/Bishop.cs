using System;
using System.Collections.Generic;

namespace ChessAI.ChessPieces
{
    public class Bishop : IChessPiece
    {
        public Bishop(Team team)
        {
            Texture = TextureAlias.GetTexture(Piece.Bishop, team);
            Team = team;
            Moved = true;
        }

        public int Worth { get => 300; }

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

        public IChessPiece Copy()
        {
            return new Bishop(Team);
        }

        bool IChessPiece.CanMoveInternal(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            if (
                chessMove.To.X != chessMove.From.X && chessMove.To.Y != chessMove.From.Y &&
                Math.Abs(chessMove.To.X - chessMove.From.X) == Math.Abs(chessMove.To.Y - chessMove.From.Y)
            )
            {
                int currentIndex = chessMove.From.ToIndex();
                int toIndex = chessMove.To.ToIndex();

                if (chessMove.To.X > chessMove.From.X)
                    if (chessMove.To.Y > chessMove.From.Y)
                        while (toIndex - currentIndex > 11)
                        {
                            currentIndex += 11;

                            if (chessBoard[currentIndex] != null)
                                return false;
                        }
                    else
                        while (currentIndex - toIndex > 11)
                        {
                            currentIndex -= 9;

                            if (chessBoard[currentIndex] != null)
                                return false;
                        }
                else
                    if (chessMove.To.Y > chessMove.From.Y)
                    while (toIndex - currentIndex > 11)
                    {
                        currentIndex += 9;

                        if (chessBoard[currentIndex] != null)
                            return false;
                    }
                else
                    while (currentIndex - toIndex > 11)
                    {
                        currentIndex -= 11;

                        if (chessBoard[currentIndex] != null)
                            return false;
                    }

                return true;
            }

            return false;
        }

        List<ChessPosition> IChessPiece.GetValidMoves(in IChessPiece[] chessBoard, ChessPosition position)
        {
            List<ChessPosition> validMoves = new List<ChessPosition>();

            int newX = position.X + 1;
            int newY = position.Y + 1;

            while (newX < 8 && newY < 8)
            {
                if (!IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(newX, newY))))
                    break;

                newX++;
                newY++;
            }

            newX = position.X - 1;
            newY = position.Y - 1;

            while (newX >= 0 && newY >= 0)
            {
                if (!IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(newX, newY))))
                    break;

                newX--;
                newY--;
            }

            newX = position.X + 1;
            newY = position.Y - 1;

            while (newX < 8 && newY >= 0)
            {
                if (!IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(newX, newY))))
                    break;

                newX++;
                newY--;
            }

            newX = position.X - 1;
            newY = position.Y + 1;

            while (newX >= 0 && newY < 8)
            {
                if (!IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(newX, newY))))
                    break;

                newX--;
                newY++;
            }

            return validMoves;
        }
    }
}