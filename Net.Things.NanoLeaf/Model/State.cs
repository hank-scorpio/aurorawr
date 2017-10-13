namespace Net.Things.NanoLeaf
{
    public class State
    {
        public Toggle On { get; set; }
        public Level Brightness { get; set; }
        public Level Hue { get; set; }
        public Level Sat { get; set; }

        public string ColorMode { get; set; }
    }
}
