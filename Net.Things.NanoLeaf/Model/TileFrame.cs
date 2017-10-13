namespace Net.Things.NanoLeaf
{
    public struct TileFrame
    {
        public TileColor Color;
        public byte Duration;

        public const int PeriodMs = 100;

        public TileFrame(TileColor c, int durationMs = PeriodMs)
        {
            Color = c;
            Duration = (byte)(durationMs / (float)PeriodMs);
        }
        public static implicit operator TileColor(TileFrame f)
            => f.Color;

        public static implicit operator TileFrame(TileColor c)
            => new TileFrame(c);

        public override string ToString()
            => $"{Color} x {Duration}";


    }
}
