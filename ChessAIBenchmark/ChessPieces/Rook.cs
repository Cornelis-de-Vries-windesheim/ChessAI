using System.Collections.Generic;

namespace ChessAIBenchmark.ChessPieces
{
    public class Rook : IChessPiece
    {
        public Rook(Team team, bool moved = false)
        {
            Texture = TextureAlias.GetTexture(Piece.Rook, team);
            Team = team;
            Moved = moved;
        }

        public int Worth { get => 500; }

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
            return new Rook(Team, Moved);
        }

        bool IChessPiece.CanMoveInternal(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            if (!Moved)
            {
                if (chessMove.To.Y == 0)
                {
                    if (chessBoard[40] != null && !chessBoard[40].Moved)
                    {
                        if (chessMove.To.X == 3)
                        {
                            for (int i = 1; i < 4; i++)
                            {
                                if (chessBoard[i] != null)
                                    return false;
                            }

                            return true;
                        }
                        else if (chessMove.To.X == 5)
                        {
                            for (int i = 6; i > 4; i--)
                            {
                                if (chessBoard[i] != null)
                                    return false;
                            }

                            return true;
                        }
                    }
                }
                else if (chessMove.To.Y == 7)
                {
                    if (chessBoard[47] != null && !chessBoard[47].Moved)
                    {
                        if (chessMove.To.X == 3)
                        {
                            for (int i = 71; i < 74; i++)
                            {
                                if (chessBoard[i] != null)
                                    return false;
                            }

                            return true;
                        }
                        else if (chessMove.To.X == 5)
                        {
                            for (int i = 76; i > 74; i--)
                            {
                                if (chessBoard[i] != null)
                                    return false;
                            }

                            return true;
                        }
                    }
                }
            }

            int toIndex = chessMove.To.ToIndex();
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