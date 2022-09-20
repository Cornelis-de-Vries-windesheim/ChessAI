using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ChessAIBenchmark;

var summary = BenchmarkRunner.Run<Benchmarks>();

[MemoryDiagnoser]
public class Benchmarks
{
    [Benchmark]
    public void ChessAIBenchmark5()
    {
        ChessAI chessAI = new ChessAI(Team.White);

        IChessPiece[] chessBoard = new IChessPiece[78];

        ChessboardHelper.IntializeChessBoard(chessBoard);

        chessAI.GetBestMove(chessBoard, Team.White, 5);
    }

    //[Benchmark]
    //public void ChessAIBenchmark4()
    //{
    //    ChessAI chessAI = new ChessAI(Team.White);

    //    IChessPiece[] chessBoard = new IChessPiece[78];

    //    ChessboardHelper.IntializeChessBoard(chessBoard);

    //    chessAI.GetBestMove(chessBoard, Team.White, 4);
    //}

    //[Benchmark]
    //public void ChessAIBenchmark3()
    //{
    //    ChessAI chessAI = new ChessAI(Team.White);

    //    IChessPiece[] chessBoard = new IChessPiece[78];

    //    ChessboardHelper.IntializeChessBoard(chessBoard);

    //    chessAI.GetBestMove(chessBoard, Team.White, 3);
    //}
}