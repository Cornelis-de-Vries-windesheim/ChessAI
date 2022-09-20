namespace ChessAI
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

        public int ToIndex()
        {
            return (Y * 10) + X;
        }

        public static int CreateIndex(int x, int y)
        {
            return (y * 10) + x;
        }

        public int X;
        public int Y;
    }
}