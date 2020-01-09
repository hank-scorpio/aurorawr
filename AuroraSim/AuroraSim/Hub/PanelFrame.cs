using Microsoft.Owin;

[assembly: OwinStartup(typeof(AuroraSim.Startup))]

namespace AuroraSim
{
    public class PanelFrame
    {
        public PanelFrame(byte id, byte r, byte g, byte b, byte w = 0, byte t = 0)
        {
            Id = id;
            R = r;
            G = g;
            B = b;
            W = w;
            T = t;
        }

        public byte     Id          { get; }
        public byte     R           { get; }
        public byte     G           { get; }
        public byte     B           { get; }
        public byte     W           { get; }
        public byte     T           { get; }
    }
}
