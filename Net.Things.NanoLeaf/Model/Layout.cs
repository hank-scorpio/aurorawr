using Infodev.Common;
using System.Linq;
using Ext;
using Seq=Ext.Seq;
using System;
using System.Collections.Generic;

namespace Net.Things.NanoLeaf
{
    public class Layout
    {
        public Tile[] Tiles;
        public TileGrid Grid; 

        public int numPanels;
        public int sideLength;

        public class pos
        {
            public int x { get; set; }
            public int y { get; set; }
            public int o { get; set; }
            public int panelId { get; set; }

        }
  
        public pos[] positionData
        {
            set
            {
                var data = value;

                Tiles = data.Select(t => new Tile()
                {
                    X = t.x,
                    Y = t.y,
                    R = t.o,
                    Id = t.panelId,
                    Size = sideLength

                }).ToArray();

                Grid = new TileGrid(Tiles);

            }
        }



        //public string layoutData
        //{
        //    set
        //    {
        //        int[] data = value.SplitWords().Select(int.Parse).ToArray();

        //        Tiles = Seq.From(2, 4).Take(data[0])
        //           .Select(i => new Tile()
        //           {
        //               Size = data[1],
        //               Id   = data[i],
        //               X    = data[i + 1],
        //               Y    = data[i + 2],
        //               R    = data[i + 3],
        //           })
        //           .ToArray();
        //    }
        //}
    }
}
