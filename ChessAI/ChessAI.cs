using ChessAI.ChessPieces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChessAI
{
    public class ChessAI
    {
        private const int minValue = -1_000_000;

        private Team Team { get; set; }

        public ChessAI(Team team)
        {
            Team = team;
        }

        static readonly string locker = string.Empty;

        public ChessMove? GetBestMove(in IChessPiece[] chessBoard, Team team, int depth = 4)
        {
            Random randomizer = new Random();

            Team turn = team;

            IChessPiece[] internalChessBoard = chessBoard;

            ChessMove? bestMove = null;

            EvaluationStep rootEvaluationStep = PlayChessboard(chessBoard, turn);

            depth = Environment.ProcessorCount > 2 ? 4 : 3;

            Game1.movesToCalculate = rootEvaluationStep.Children.Count(item => item.Initialiazed);

            if (Game1.movesToCalculate < Environment.ProcessorCount)
                depth++;
            if (Game1.movesToCalculate < Environment.ProcessorCount / 2)
                depth++;

            int value = -1_000_000;

            int alpha = minValue;
            const int beta = 1_000_000;

            _ = Parallel.For(0, rootEvaluationStep.Children.Length, (i) =>
            {
                EvaluationStep evaluationStep = rootEvaluationStep.Children[i];
                if (evaluationStep.Initialiazed)
                {
                    int childValue = Minimax(evaluationStep, depth - 1, alpha, beta, false);

                    lock (locker)
                    {
                        Game1.movesToCalculate--;
                        if (childValue > value)
                        {
                            value = childValue;
                            bestMove = evaluationStep.ChessMove;
                            Game1.MovingChessPiece.Item1 = internalChessBoard[bestMove.Value.From.ToIndex()] ?? Game1.MovingChessPiece.Item1;
                            Game1.MovingChessPiece.Item2 = new Vector2(bestMove.Value.To.X * 150, bestMove.Value.To.Y * 150);
                            Game1.optimisim = value;
                            Game1.pesimism = alpha;
                        } else if (childValue == value && randomizer.Next(1, 3) == 1)
                        {
                            value = childValue;
                            bestMove = evaluationStep.ChessMove;
                            Game1.MovingChessPiece.Item1 = internalChessBoard[bestMove.Value.From.ToIndex()] ?? Game1.MovingChessPiece.Item1;
                            Game1.MovingChessPiece.Item2 = new Vector2(bestMove.Value.To.X * 150, bestMove.Value.To.Y * 150);
                            Game1.optimisim = value;
                            Game1.pesimism = alpha;
                        }
                        alpha = alpha > value ? alpha : value;
                    }
                }
            });

            return bestMove;
        }

        public int Minimax(EvaluationStep evaluationStep, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            if (evaluationStep.Initialiazed)
            {
                PlayChessboard(ref evaluationStep, maximizingPlayer ? Team : Team == Team.White ? Team.Black : Team.White, depth > 1);

                if (depth <= 0)
                    return GetWorthPieces(evaluationStep.ChessBoard, Team);

                if (maximizingPlayer)
                {
                    int value = -1_000_000;

                    for (int i = 0; i < evaluationStep.Children.Length; i++)
                    {
                        EvaluationStep child = evaluationStep.Children[i];
                        if (child.Initialiazed)
                        {
                            int childValue = Minimax(child, depth - 1, alpha, beta, false);
                            value = childValue > value ? childValue : value;
                            if (value > beta)
                                break;
                            alpha = alpha > value ? alpha : value;
                        }
                    }

                    return value;
                }
                else
                {
                    int value = 1_000_000;

                    for (int i = 0; i < evaluationStep.Children.Length; i++)
                    {
                        EvaluationStep child = evaluationStep.Children[i];
                        if (child.Initialiazed)
                        {
                            int childValue = Minimax(child, depth - 1, alpha, beta, true);
                            value = childValue < value ? childValue : value;
                            beta = beta < value ? beta : value;
                            if (value < alpha)
                                break;
                            beta = beta < value ? beta : value;
                        }
                    }

                    return value;
                }
            }

            return maximizingPlayer ? -1_000_000 : 1_000_000;
        }

        public EvaluationStep PlayChessboard(in IChessPiece[] chessBoard, Team turn)
        {
            EvaluationStep evaluationStep = new();
            evaluationStep.Children = new EvaluationStep[100];

            int moveIndex = 0;

            foreach (ChessMove chessMove in ChessboardHelper.GetAllValidMoves(chessBoard, turn))
            {
                if (moveIndex == evaluationStep.Children.Length)
                {
                    EvaluationStep[] newChildren = new EvaluationStep[moveIndex + 50];
                    evaluationStep.Children.CopyTo(newChildren, 0);
                    evaluationStep.Children = newChildren;
                }

                IChessPiece[] newChessBoard = new IChessPiece[chessBoard.Length];
                chessBoard.CopyTo(newChessBoard, 0);

                switch (newChessBoard[chessMove.From.ToIndex()].GetType().Name)
                {
                    case "Rook":
                        newChessBoard[chessMove.From.ToIndex()] = newChessBoard[chessMove.From.ToIndex()].Copy();
                        break;
                    case "Pawn":
                        newChessBoard[chessMove.From.ToIndex()] = newChessBoard[chessMove.From.ToIndex()].Copy();
                        break;
                    case "King":
                        if (chessMove.From.X == 4)
                        {
                            newChessBoard[chessMove.From.ToIndex()] = newChessBoard[chessMove.From.ToIndex()].Copy();
                            if (chessMove.From.Y == 0)
                            {
                                if (newChessBoard[00] is not null)
                                    newChessBoard[00] = newChessBoard[00].Copy();
                                if (newChessBoard[07] is not null)
                                    newChessBoard[07] = newChessBoard[07].Copy();
                            }
                            else if (chessMove.From.Y == 7)
                            {
                                if (newChessBoard[70] is not null)
                                    newChessBoard[70] = newChessBoard[70].Copy();
                                if (newChessBoard[77] is not null)
                                    newChessBoard[77] = newChessBoard[77].Copy();
                            }

                        }
                        break;

                }

                newChessBoard[chessMove.From.ToIndex()].Move(newChessBoard, chessMove);

                evaluationStep.Children[moveIndex] = new EvaluationStep(chessMove, newChessBoard);
                moveIndex++;
            }

            return evaluationStep;
        }

        public void PlayChessboard(ref EvaluationStep evaluationStep, Team turn, bool optimize = true)
        {

            if (evaluationStep.Initialiazed)
            {
                evaluationStep.Children = new EvaluationStep[100];
                int moveIndex = 0;

                foreach (ChessMove chessMove in ChessboardHelper.GetAllValidMoves(evaluationStep.ChessBoard, turn, optimize)) {
                    if (moveIndex == evaluationStep.Children.Length)
                    {
                        EvaluationStep[] newChildren = new EvaluationStep[moveIndex + 50];
                        evaluationStep.Children.CopyTo(newChildren, 0);
                        evaluationStep.Children = newChildren;

                    }

                    IChessPiece[] newChessBoard = new IChessPiece[evaluationStep.ChessBoard.Length];
                    evaluationStep.ChessBoard.CopyTo(newChessBoard, 0);

                    switch (newChessBoard[chessMove.From.ToIndex()].GetType().Name)
                    {
                        case "Rook":
                            newChessBoard[chessMove.From.ToIndex()] = newChessBoard[chessMove.From.ToIndex()].Copy();
                            break;
                        case "Pawn":
                            newChessBoard[chessMove.From.ToIndex()] = newChessBoard[chessMove.From.ToIndex()].Copy();
                            break;
                        case "King":
                            if (chessMove.From.X == 4)
                            {
                                newChessBoard[chessMove.From.ToIndex()] = newChessBoard[chessMove.From.ToIndex()].Copy();
                                if (chessMove.From.Y == 0)
                                {
                                    if (newChessBoard[00] is not null)
                                        newChessBoard[00] = newChessBoard[00].Copy();
                                    if (newChessBoard[07] is not null)
                                        newChessBoard[07] = newChessBoard[07].Copy();
                                } else if (chessMove.From.Y == 7)
                                {
                                    if (newChessBoard[70] is not null)
                                        newChessBoard[70] = newChessBoard[70].Copy();
                                    if (newChessBoard[77] is not null)
                                        newChessBoard[77] = newChessBoard[77].Copy();
                                }

                            }
                            break;

                    }

                    newChessBoard[chessMove.From.ToIndex()].Move(newChessBoard, chessMove);

                    evaluationStep.Children[moveIndex] = new EvaluationStep(evaluationStep.ChessMove, newChessBoard);
                    moveIndex++;
                }
            }
        }

        static public int GetWorthPieces(in IChessPiece[] chessBoard, Team team)
        {
            int score = 0;

            if (chessBoard != null)
            {
                int position = 0;
                foreach (IChessPiece chessPiece in chessBoard)
                {
                    if (chessPiece != null && score < 20_000_000 && score > -20_000_000)
                    {
                        if (chessPiece is Pawn)
                            score += team == chessPiece.Team ? (chessPiece.Worth
                                + (((Pawn)chessPiece).Forward ? position : 78 - position))
                                :
                                -(chessPiece.Worth
                                + (((Pawn)chessPiece).Forward ? position : 78 - position));
                        else
                            score += team == chessPiece.Team ? chessPiece.Worth : -chessPiece.Worth;
                    }

                    position++;
                }
            }

            return score;
        }
    }

    public struct EvaluationStep
    {
        public EvaluationStep(ChessMove chessMove, IChessPiece[] chessBoard)
        {
            ChessMove = chessMove;
            ChessBoard = chessBoard;
            Initialiazed = true;
            Children = null;
        }

        public readonly bool Initialiazed;
        public readonly ChessMove ChessMove;
        public readonly IChessPiece[] ChessBoard;
        public EvaluationStep[] Children;
    }

    public struct ChessMove
    {
        public ChessMove(ChessPosition from, ChessPosition to)
        {
            From = from;
            To = to;
        }

        public ChessPosition From;
        public ChessPosition To;
    }
}