using Ext;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections;
using Infodev.Common;

namespace Net.Things.NanoLeaf
{
    public class TileGrid : IEnumerable<Tile>
    {
        public TileRank X { get; }
        public TileRank Y { get; }

        public IEnumerable<Tile> XY => X.SelectMany(x => x);
        public IEnumerable<Tile> YX => Y.SelectMany(y => y);


        public TileGrid(Tile[] tiles)
        {
            SetRank(tiles);
            var x = new TileRank(tiles, t => t.X);
            var y = new TileRank(tiles, t => t.Y);

            bool swap = (x.Length < y.Length);
            X = swap ? y : x;
            Y = swap ? x : y;
        }

        public Tile this[int x, int y]
            => X[x][y];

        public IEnumerator<Tile> GetEnumerator() 
            => (XY as IEnumerable<Tile>).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() 
            => XY.GetEnumerator();

        static IEnumerable<int> Rank<T>(IEnumerable<T> seq) where T : IComparable<T>
            => seq.Order().Distinct().ToArray().Apply(rank => seq.Select(rank.IndexOf));


        static void SetRank(Tile[] tiles)
        {
            var rx = Rank(tiles.Select(t => t.X)).ToArray();
            var ry = Rank(tiles.Select(t => t.Y)).ToArray();

            for (int n = 0; n < tiles.Length; n++)
            {
                tiles[n].RX = rx[n];
                tiles[n].RY = ry[n];
            }

        }
    }
}
