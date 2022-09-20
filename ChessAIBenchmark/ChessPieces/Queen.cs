using System;
using System.Collections.Generic;

namespace ChessAIBenchmark.ChessPieces
{
    public class Queen : IChessPiece
    {
        public Queen(Team team)
        {
            Texture = TextureAlias.GetTexture(Piece.Queen, team);
            Team = team;
            Moved = true;
        }

        public int Worth { get => 930; }

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
            return new Queen(Team);
        }

        bool IChessPiece.CanMoveInternal(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            int toIndex = chessMove.To.ToIndex();
            if (
                chessMove.To.X != chessMove.From.X && chessMove.To.Y != chessMove.From.Y &&
                Math.Abs(chessMove.To.X - chessMove.From.X) == Math.Abs(chessMove.To.Y - chessMove.From.Y)
            )
            {
                int currentIndex = chessMove.From.ToIndex();

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

            if (chessMove.To.X != chessMove.From.X && chessMove.To.Y == chessMove.From.Y)
            {
                if (chessMove.To.X > chessMove.From.X)
                {
                    for (int i = chessMove.From.X + 1 + (chessMove.To.Y * 10); i < toIndex; i += 1)
                    {
                        if (chessBoard[i] != null)
                            return false;
                    }
                }
                else
                {
                    for (int i = chessMove.From.X - 1 + (chessMove.To.Y * 10); i > toIndex; i--)
                    {
                        if (chessBoard[i] != null)
                            return false;
                    }
                }

                return true;
            }
            else if (chessMove.To.Y != chessMove.From.Y && chessMove.To.X == chessMove.From.X)
            {
                if (chessMove.To.Y > chessMove.From.Y)
                {
                    for (int i = (chessMove.From.Y * 10) + 10 + chessMove.To.X; i < toIndex; i += 10)
                    {
                        if (chessBoard[i] != null)
                            return false;
                    }
                }
                else
                {
                    for (int i = (chessMove.From.Y * 10) - 10 + chessMove.To.X; i > toIndex; i -= 10)
                    {
                        if (chessBoard[i] != null)
                            return false;
                    }
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

            if (position.X != 0)
            {
                for (int i = position.X - 1; i >= 0; i--)
                {
                    if (!IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(i, position.Y))))
                        break;
                }
            }
            for (int i = position.X + 1; i < 8; i++)
            {
                if (!IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(i, position.Y))))
                    break;
            }
            if (position.Y != 0)
            {
                for (int i = position.Y - 1; i >= 0; i--)
                {
                    if (!IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X, i))))
                        break;
                }
            }
            for (int i = position.Y + 1; i < 8; i++)
            {
                if (!IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X, i))))
                    break;
            }

            return validMoves;
        }
    }
}