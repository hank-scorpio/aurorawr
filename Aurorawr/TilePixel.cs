using Ext;
using static System.Windows.Media.Colors;
using System.Windows.Media;
using Net.Things.NanoLeaf;
using System.Collections.Generic;

namespace Autorawr
{
    public class TilePixel
    {
        static Color[] BandPalette 
            = { DeepSkyBlue, LimeGreen, Yellow, Orange, Red };

        static int[] BandColorMap 
            = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4 };

        static Color GetBandColor(int i)
            => BandPalette[BandColorMap[i]];

        public TileColor Color
        {
            get => Tile.Color;
            set { if (Tile.Color != value) { Time = 50; } Tile.Color = value; }
        }
        public Tile Tile    { get; }
        public int  Col     { get; }
        public int  Row     { get; }
        public int  Time    { get; set; }






        int streak = -1;
  


        public void Update(double max, double under, int duration)
        {
            if ((Time -= duration) > 0) return;



            // OVER PEAK -- COLORED PART
            if (under > 0)
            {
                // UNDER -> OVER
                if (streak < 0)
                {
                    streak = 0;

                    // Set band color
                    Color = GetBandColor(Col);

                    //if ((streak == 0 && under > 3) || (streak > 10 && under > 5))
                    //{
                    //    if (Rnd.Binary(0.95)) Color.Add(224);
                    //}

                    // Row-level variatons
                    if (Row == 0) Color = Color.Add(60, 0, -30);
                    if (Row == 1) Color = Color.Add(0, -60, 30);
                    if (Row == 2) Color = Color.Add(-60, 120, 0);
                }
                else if (streak > 6 && max < 7 && under > 3)
                {
                    if (Rnd.Binary(0.95))
                    {
                        // Color = GetBandColor(Col);
                        Color = GetBandColor(Col + 1);
                        Color.Add(128);

                    }
                }
                    streak++;
            }

            // UNDER PEAK -- DIM PART
            else
            {
             
                // OVER -> UNDER
                if (streak > 0) 
                {
                    streak = 0;
                }


                if (streak < 6 && max > 7 && under < 2)
                {
                    if (Rnd.Binary(0.95))
                    {
                        // Color = GetBandColor(Col);
                        Color = GetBandColor(Col - 1);
                        Color.Add(64);

                    }
                     
                }
         

                // Dimming (w/ variatons) if not already black
                else if (!Color.IsBlack) 
                {
                    byte dim = 18;

                    if (under < -2.5)       dim *= 3;
                    else if (streak < -1)        dim *= 2;
        
                    if (streak < -2 && Col<4)        dim *= 2;
               //     if (Rnd.Binary(0.9))   dim *= 2;
                  
                    Color -= dim;
                }
                streak--;
            }
        
        }


        public TilePixel(Tile t, int col, int row)
        {
            Tile = t;
            Col = col;
            Row = row;
        }
    }
}
