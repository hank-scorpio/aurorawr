using Ext;
using System.Linq;
using System.Windows.Media;
using System.IO;
using System.Collections.Generic;
using System;
using Tiles = System.Collections.Generic.IEnumerable<Net.Things.NanoLeaf.Tile>;

namespace Net.Things.NanoLeaf
{
    public static class TileEnumerableExt
    {
        public static Tiles Color(this Tiles tiles, Func<Tile, TileColor> picker)
        {
            foreach (var t in tiles)
            {
                t.Color = picker(t);
            }
            return tiles;
        }


        public static Tiles Color(this Tiles tiles, Func<TileColor, TileColor> picker)
            => tiles.Color(t => picker(t.Color));

        public static Tiles Color(this Tiles tiles, Func<TileColor> picker)
            => tiles.Color((Tile t) => picker());
         
        public static Tiles Color(this Tiles tiles, TileColor c)
            => tiles.Color(() => c);
    }
    public class Tile
    {
        public int Size;
        public int Id;

        public int X;
        public int Y;
        public int R;

        public int RX;
        public int RY;

        public IEnumerable<TileFrame> Frames => new [] { frame } ;

        TileFrame frame;
        TileFrame? lastFrameWritten;

        public int Duration
        {
            get => frame.Duration;
            set => frame.Duration = (byte)value;
        }
        public TileColor Color
        {
            get => frame;
            set => frame.Color = value;
        }

        public override string ToString()
            => $"[id:{Id}] x:{X}, y:{Y}, r:{R}";


        public bool HasChanges
        { 
            get => !frame.Equals(lastFrameWritten);
            set => lastFrameWritten = (value ? null : (TileFrame?)frame);
        }

    }
} 
