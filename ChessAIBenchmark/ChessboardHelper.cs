using ChessAIBenchmark.ChessPieces;
using System.Collections.Generic;
using System.Linq;

namespace ChessAIBenchmark
{
    internal static class ChessboardHelper
    {
        public static IEnumerable<ChessMove> GetAllValidMoves(IChessPiece[] chessBoard, Team team, bool optimize = true)
        {
            List<ChessMove> validChessPiecesMoves = new List<ChessMove>();

            int index = 0;
            foreach (IChessPiece chessPiece in chessBoard)
            {
                if (chessPiece != null && chessPiece.Team == team)
                {
                    ChessPosition chessPosition = new ChessPosition(index);
                    List<ChessPosition> validMoves = chessPiece.GetValidMoves(chessBoard, chessPosition);

                    foreach (ChessPosition ToChessPosition in validMoves)
                    {
                        validChessPiecesMoves.Add(new ChessMove(chessPosition, ToChessPosition));
                    }
                }
                index++;
            }

            if (optimize) {
                return validChessPiecesMoves.OrderBy((chessMove) =>
                {
                    int scoreTo = chessBoard[chessMove.To.ToIndex()]?.Worth ?? 0;
                    int scoreFrom = chessBoard[chessMove.From.ToIndex()]?.Worth ?? -1_000_000;

                    if (chessBoard[chessMove.From.ToIndex()].UnderAttack(chessBoard, chessMove.From))
                    {
                        scoreTo = 2 * scoreTo;
                    }

                    return scoreTo - scoreFrom;
                });
            } else
                return validChessPiecesMoves;
        }

        public static void IntializeChessBoard (in IChessPiece[] chessBoard) {
            chessBoard[0] = new Rook(Team.Black);
            chessBoard[1] = new Knight(Team.Black);
            chessBoard[2] = new Bishop(Team.Black);
            chessBoard[3] = new Queen(Team.Black);
            chessBoard[4] = new King(Team.Black);
            chessBoard[5] = new Bishop(Team.Black);
            chessBoard[6] = new Knight(Team.Black);
            chessBoard[7] = new Rook(Team.Black);

            chessBoard[10] = new Pawn(Team.Black, false);
            chessBoard[11] = new Pawn(Team.Black, false);
            chessBoard[12] = new Pawn(Team.Black, false);
            chessBoard[13] = new Pawn(Team.Black, false);
            chessBoard[14] = new Pawn(Team.Black, false);
            chessBoard[15] = new Pawn(Team.Black, false);
            chessBoard[16] = new Pawn(Team.Black, false);
            chessBoard[17] = new Pawn(Team.Black, false);

            chessBoard[60] = new Pawn(Team.White, true);
            chessBoard[61] = new Pawn(Team.White, true);
            chessBoard[62] = new Pawn(Team.White, true);
            chessBoard[63] = new Pawn(Team.White, true);
            chessBoard[64] = new Pawn(Team.White, true);
            chessBoard[65] = new Pawn(Team.White, true);
            chessBoard[66] = new Pawn(Team.White, true);
            chessBoard[67] = new Pawn(Team.White, true);

            chessBoard[70] = new Rook(Team.White);
            chessBoard[71] = new Knight(Team.White);
            chessBoard[72] = new Bishop(Team.White);
            chessBoard[73] = new Queen(Team.White);
            chessBoard[74] = new King(Team.White);
            chessBoard[75] = new Bishop(Team.White);
            chessBoard[76] = new Knight(Team.White);
            chessBoard[77] = new Rook(Team.White);
        }
    }
}