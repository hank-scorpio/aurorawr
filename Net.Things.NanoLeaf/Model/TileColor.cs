using Infodev.Common;
using System;
using System.Windows.Media;
using static System.Math;
namespace Net.Things.NanoLeaf
{
    public struct TileColor : IEquatable<TileColor>
    {
        public byte R;
        public byte G;
        public byte B;
        public byte W;

        public bool IsBlack     => Intensity == 0;
        public int Intensity    => R + G + B;

        public TileColor(int r, int g, int b, int w = 0)
        {
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
            W = (byte)w;
        }

        public TileColor(Color c) : this(c.R, c.G, c.B, 0)
        {
        }
        public TileColor(uint rgb) 
            : this((byte)(rgb >> 16), (byte)(rgb >> 8), (byte)(rgb))
        {
        }
        public TileColor Add(int r, int g, int b)
            => new TileColor((R + r).Bound(0, 255), (G + g).Bound(0, 255), (B + b).Bound(0, 255));
        
        public TileColor Add(int val)
        => Add(val, val, val);
  
        public TileColor Add(TileColor c)
        => Add(c.R, c.G, c.B);
        
        public TileColor Subtract(TileColor c)
        => Add(-c.R, -c.G, -c.B);
        public TileColor Subtract(byte val)
        => Add(-val);
        public TileColor Subtract(byte r, byte g, byte b)
        => Add(-r, -g, -b);

        public override string ToString()
            => $"({R}, {G}, {B})";
        public static implicit operator Color(TileColor c)
            => Color.FromRgb(c.R, c.G, c.B);
        public static implicit operator TileColor(uint rgb)
            => new TileColor(rgb);
        public static implicit operator TileColor(Color c)
            => new TileColor(c);
        public static bool operator ==(TileColor x, TileColor y)
            => x.Equals(y);
        public static bool operator !=(TileColor x, TileColor y)
            => !x.Equals(y);
        public static TileColor operator -(TileColor x, TileColor y)
            => x.Subtract(y);
        public static TileColor operator +(TileColor x, TileColor y)
            => x.Add(y);
        public static TileColor operator -(TileColor x, byte i)
            => x.Subtract(i,i,i);
        public static TileColor operator +(TileColor x, byte i)
            => x.Add(i,i,i);
        public override bool Equals(object c)
            => c is TileColor && this.Equals((TileColor)c);
        public bool Equals(TileColor c)
            => this.R == c.R
            && this.G == c.G
            && this.B == c.B;
    }
}
