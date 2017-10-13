using Net.Things.NanoLeaf;
using System;
using static System.Windows.Media.Colors;
using System.Windows.Media;

namespace Tests
{
    class MyTileStreamer : TileStreamer
    {
        int tick = 0;
   
        

        public override int Period => 250;

        Tile[] XI(int i = 0) => X[(tick + i) % X.Length];

        public override void Flushing(AuroraUdpStreamer sender, Layout layout)
        {



            Grid.Color(c => c.Subtract(64));

            XI().Color(Red);


         

            Console.WriteLine(tick++);
        }


    }
}
