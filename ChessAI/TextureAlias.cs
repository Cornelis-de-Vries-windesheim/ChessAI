namespace ChessAI
{
    public static class TextureAlias
    {
        private static readonly char[] pieceValues = new char[] {
            'k',
            'q',
            'b',
            'n',
            'p',
            'r'
        };

        private static readonly char[] teamValues = new char[] {
            'b',
            'w'
        };

        public static string GetTexture(Piece piece, Team team)
        {
            return teamValues[(int)team].ToString() + pieceValues[(int)piece].ToString();
        }
    }

    public enum Piece
    {
        King,
        Queen,
        Bishop,
        Knight,
        Pawn,
        Rook
    }

    public enum Team
    {
        Black,
        White,
    }
}