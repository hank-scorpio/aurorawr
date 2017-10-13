namespace Net.Things.NanoLeaf
{
    public class Toggle
    {
        public bool Value { get; set; }

        public static implicit operator bool(Toggle x)
            => x.Value;
        public override string ToString()
            => $"{Value}";
    }
}
