using System;
using System.Collections.Generic;

namespace ChessAIBenchmark.ChessPieces
{
    public class Knight : IChessPiece
    {
        public int Worth { get => 300; }

        public Knight(Team team)
        {
            Texture = TextureAlias.GetTexture(Piece.Knight, team);
            Team = team;
            Moved = true;
        }

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
            return new Knight(Team);
        }

        bool IChessPiece.CanMoveInternal(in IChessPiece[] chessBoard, ChessMove chessMove)
        {
            int absX = Math.Abs(chessMove.To.X - chessMove.From.X);
            int absY = Math.Abs(chessMove.To.Y - chessMove.From.Y);

            if (
                absX == 2 && absY == 1 ||
                absX == 1 && absY == 2
            )
            {
                return true;
            }

            return false;
        }

        List<ChessPosition> IChessPiece.GetValidMoves(in IChessPiece[] chessBoard, ChessPosition position)
        {
            List<ChessPosition> validMoves = new List<ChessPosition>();

            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X - 2, position.Y - 1)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X - 1, position.Y - 2)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X + 2, position.Y + 1)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X + 1, position.Y + 2)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X + 2, position.Y - 1)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X + 1, position.Y - 2)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X - 2, position.Y + 1)));
            IChessPiece.Displace(chessBoard, validMoves, new ChessMove(position, new ChessPosition(position.X - 1, position.Y + 2)));

            return validMoves;
        }
    }
}