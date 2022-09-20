namespace ChessAIBenchmark
{
    public struct ChessPosition
    {
        public ChessPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public ChessPosition(int index)
        {
            Y = index / 10;
            X = index - (Y * 10);
        }

        private int index = -1;

        public int ToIndex()
        {
            if (index == -1)
                index = (Y * 10) + X;

            return index;
        }

        public static int CreateIndex(int x, int y)
        {
            return (y * 10) + x;
        }

        public int X;
        public int Y;
    }
}