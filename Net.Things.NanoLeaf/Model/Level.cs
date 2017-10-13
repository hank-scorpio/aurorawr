using Infodev.Common;

namespace Net.Things.NanoLeaf
{
    public class Level
    {
        public int Value { get; set; }

        public int Min { get; set; }
        public int Max { get; set; }


        public int Range => Max - Min;

        public double ValuePct => (Value - Min) / (double)Range;

        public int Clamp(int value) => value.Bound(Min, Max);

        public override string ToString()
            => $"{ValuePct:0.00%}";

        public static implicit operator int(Level x)
            => x.Value;
        public static implicit operator double(Level x)
            => x.ValuePct;
    }
}
