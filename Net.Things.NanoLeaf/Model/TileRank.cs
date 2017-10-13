using Ext;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Net.Things.NanoLeaf
{
    public class TileRank : IEnumerable<Tile[]>
    {
        Tile[][] groups;
        Func<Tile, int> rankSelector;

        int RankOf(Tile tile) => rankSelector(tile);

        public TileRank(Tile[] tiles, Func<Tile, int> rankSelector)
        {
            this.rankSelector = rankSelector;
            this.groups = tiles.GroupBy(rankSelector).OrderBy(x=> x. Key).Select(x => x.ToArray()).ToArray();

        }

        public Tile[] this[int rank]
            => groups[rank];

        public int Width 
            => groups.Select(x => x.Length).Max();
        public int Length 
            => groups.Length;


        public IEnumerator<Tile[]> GetEnumerator() 
            => (groups as IEnumerable<Tile[]>).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => groups.GetEnumerator();
    }
}
