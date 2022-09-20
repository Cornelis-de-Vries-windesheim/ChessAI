using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;

namespace ChessAI
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}