using System;
using System.Collections.Generic;

namespace ChessAIBenchmark.ChessPieces
{
    public class King : IChessPiece
    {
        public King(Team team, bool moved = false)
        {
            Texture = TextureAlias.GetTexture(Piece.King, team);
            Team = team;
            Moved = moved;
        }

        public int Worth { get => 1_000_000; }

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
            return new King(Team, Moved);
        }

        void IChessPiece.Move(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            if (!Moved)
            {
                if (chessMove.To.Y == 0)
                {
                    if (chessMove.To.X == 2 && chessBoard[ChessPosition.CreateIndex(0, 0)] != null && !chessBoard[ChessPosition.CreateIndex(0, 0)].Moved)
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            if (chessBoard[ChessPosition.CreateIndex(i, 0)] != null)
                                return;
                        }

                        chessBoard[ChessPosition.CreateIndex(0, 0)].Move(chessBoard, new ChessMove(new ChessPosition(0, 0), new ChessPosition(3, 0)));
                    }
                    else if (chessMove.To.X == 6 && chessBoard[ChessPosition.CreateIndex(7, 0)] != null && !chessBoard[ChessPosition.CreateIndex(7, 0)].Moved)
                    {
                        for (int i = 6; i > 4; i--)
                        {
                            if (chessBoard[ChessPosition.CreateIndex(i, 0)] != null)
                                return;
                        }

                        chessBoard[ChessPosition.CreateIndex(7, 0)].Move(chessBoard, new ChessMove(new ChessPosition(7, 0), new ChessPosition(5, 0)));
                    }
                }
                else if (chessMove.To.Y == 7)
                {
                    if (chessMove.To.X == 2 && chessBoard[ChessPosition.CreateIndex(0, 7)] != null && !chessBoard[ChessPosition.CreateIndex(0, 7)].Moved)
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            if (chessBoard[ChessPosition.CreateIndex(i, 7)] != null)
                                return;
                        }

                        chessBoard[ChessPosition.CreateIndex(0, 7)].Move(chessBoard, new ChessMove(new ChessPosition(0, 7), new ChessPosition(3, 7)));
                    }
                    else if (chessMove.To.X == 6 && chessBoard[ChessPosition.CreateIndex(7, 7)] != null && !chessBoard[ChessPosition.CreateIndex(7, 7)].Moved)
                    {
                        for (int i = 6; i > 4; i--)
                        {
                            if (chessBoard[ChessPosition.CreateIndex(i, 7)] != null)
                                return;
                        }

                        chessBoard[ChessPosition.CreateIndex(7, 7)].Move(chessBoard, new ChessMove(new ChessPosition(7, 7), new ChessPosition(5, 7)));
                    }
                }
            }
            Moved = true;
            int fromIndex = chessMove.From.ToIndex();
            IChessPiece chessPiece = chessBoard[fromIndex];

            chessBoard[fromIndex] = null;
            chessBoard[chessMove.To.ToIndex()] = chessPiece;
        }

        private bool CanMove(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            int fromIndex = chessMove.From.ToIndex();
            int toIndex = chessMove.To.ToIndex();

            if (
                (chessMove.To.X < 8 || chessMove.To.X >= 0 || chessMove.To.Y < 8 || chessMove.To.Y >= 0) &&
                (chessMove.To.X != chessMove.From.X || chessMove.To.Y != chessMove.From.Y) &&
                fromIndex < 80 && fromIndex >= 0 && chessBoard[fromIndex] != null
            )
            {
                if (chessBoard[toIndex] != null && chessBoard[toIndex].Team == chessBoard[fromIndex].Team)
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
            int absX = Math.Abs(chessMove.To.X - chessMove.From.X);
            int absY = Math.Abs(chessMove.To.Y - chessMove.From.Y);

            if (!Moved)
            {
                if (chessMove.To.Y == 0 && chessMove.From.Y == 0)
                {
                    if (chessMove.To.X == 2 && chessBoard[00] != null && !chessBoard[00].Moved)
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            if (chessBoard[ChessPosition.CreateIndex(i, 0)] != null)
                                return false;
                        }

                        return true;
                    }
                    else if (chessMove.To.X == 6 && chessBoard[70] != null && !chessBoard[70].Moved)
                    {
                        for (int i = 6; i > 4; i--)
                        {
                            if (chessBoard[ChessPosition.CreateIndex(i, 0)] != null)
                                return false;
                        }

                        return true;
                    }
                }
                else if (chessMove.To.Y == 7 && chessMove.From.Y == 7)
                {
                    if (chessMove.To.X == 2 && chessBoard[07] != null && !chessBoard[07].Moved)
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            if (chessBoard[ChessPosition.CreateIndex(i, 7)] != null)
                                return false;
                        }

                        return true;
                    }
                    else if (chessMove.To.X == 6 && chessBoard[77] != null && !chessBoard[77].Moved)
                    {
                        for (int i = 6; i > 4; i--)
                        {
                            if (chessBoard[ChessPosition.CreateIndex(i, 7)] != null)
                                return false;
                        }

                        return true;
                    }
                }
            }

            if (
                absX == 1 && absY == 1 ||
                absX == 1 && absY == 0 ||
                absX == 0 && absY == 1
            )
            {
                return true;
            }

            return false;
        }

        List<ChessPosition> IChessPiece.GetValidMoves(in IChessPiece[] chessBoard, ChessPosition position)
        {
            List<ChessPosition> validMoves = new List<ChessPosition>();

            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X + 1, position.Y)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X - 1, position.Y)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X, position.Y - 1)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X, position.Y + 1)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X + 1, position.Y - 1)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X - 1, position.Y + 1)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X + 1, position.Y + 1)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X - 1, position.Y - 1)));

            if (!Moved && !IChessPiece.AttacksKing(chessBoard, new ChessMove(position, position)))
            {
                ChessPosition towerPosition = new ChessPosition(0, position.Y);
                int towerIndex = ChessPosition.CreateIndex(0, position.Y);

                bool canMove = true;

                if (chessBoard[towerIndex] != null && !chessBoard[towerIndex].Moved && !IChessPiece.UnderAttack(chessBoard, new ChessMove(towerPosition, towerPosition)))
                {
                    for (int i = 1 + (position.Y * 10); i < 4; i++)
                    {
                        if (chessBoard[i] != null)
                            canMove = false;
                    }

                    if (canMove)
                        IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(2, position.Y)));
                }

                towerPosition = new ChessPosition(7, position.Y);

                if (chessBoard[towerIndex] != null && !chessBoard[towerIndex].Moved && !IChessPiece.UnderAttack(chessBoard, new ChessMove(towerPosition, towerPosition)))
                {
                    for (int i = 5 + (position.Y * 10); i < 7; i++)
                    {
                        if (chessBoard[i] != null)
                            canMove = false;
                    }

                    if (canMove)
                        IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(6, position.Y)));
                }
            }

            return validMoves;
        }
    }
}