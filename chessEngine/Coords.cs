namespace chessEngine
{
    public struct Coords
    {
        private readonly int _x, _y;

        public int X => _x;
        public int Y => _y;

        public Coords(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public static Coords operator +(Coords coords1, Coords coords2)
        {
            return new Coords(coords1.X + coords2.X, coords1.Y + coords2.Y);
        }

    }
}
