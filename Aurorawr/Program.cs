using Infodev.Common;
using Ext;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using static System.Windows.Media.Colors;
using Net.Things.NanoLeaf;
using System.Threading.Tasks;
using System.Collections.Generic;
using static System.Console;
namespace Autorawr
{

    class Program
    {
        static float PeakMultiplier             = 15f;
        static float PeakOffset                 = 3F;
        static int MeterSamplingBacklog = 5;
        static TimeSpan MeterSamplingInterval   = TimeSpan.FromMilliseconds(2);
        static TimeSpan PaintStreamingInterval  = TimeSpan.FromMilliseconds(30);




        static IEnumerable<int> Rank<T>(IEnumerable<T> seq) where T : IComparable<T>
            => seq.Order().Distinct().ToArray().Apply(rank => seq.Select(rank.IndexOf));



        static void Main(string[] args)
        {

            var api = Aurora.Leaf1;


            var tt = api.Layout.Tiles;
            var rx = Rank(tt.Select(t => t.X)).ToArray();
            var ry = Rank(tt.Select(t => t.Y)).ToArray();
            var tiles = tt.Select((t, i) => new TilePixel(t, ry[i], rx[i])).ToArray();





            api.Saturation = 100;
            api.Brightness = 100;

            api.SetExternalControlMode().Wait();
            api.UdpStreamer.SendFrame().Wait();

            //CapacityQueue<float> ival = new CapacityQueue<float>(100);
            //ival.Enqueue(0);

            long smpTotal = 0;
            long smpTotalLast = 0;

            long streamTotal = 0;
            PeakSamples peak = new PeakSamples(MeterSamplingBacklog);

            Stopwatch samplingInterval = Stopwatch.StartNew();
            Stopwatch streamingInterval = Stopwatch.StartNew();

            var  maxHist = new CapacityQueue<float>(60);

            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        int ms = 250;

            //        var n = (smpTotal - smpTotalLast);


            //        Console.Write($"\r smp/s: " +
            //            $"| min: {ival.Min()}, max: {ival.Max()}, var: {ival.Variance(d => d)}" +
            //            $"| avg. {ival.Average()} ms" +
            //            $"| SMP/S:{(n / 250f * 1000f),10}/s  " +
            //            $"| LAST/TOTAL:  {n} / {smpTotal}");

      

            //        smpTotalLast = smpTotal;
            //        Thread.Sleep(ms);



            //    }

            //});

            while (true)
            {
                Thread.Sleep(0);


                if (samplingInterval.Elapsed >= MeterSamplingInterval)
                {
                    smpTotal++;
                    //ival.Enqueue(samplingInterval.ElapsedMilliseconds);
                    samplingInterval.Restart();
                    peak.Enqueue();
            
                
                }
                if (streamingInterval.Elapsed >= PaintStreamingInterval)
                {
                    streamTotal++;
                    streamingInterval.Restart();
                    // var m = 100 * peak.Max();
                    //// m = maxHist.Max();
                    // Console.Write($"\r[ {m:00.0} ] {"".PadRight((int)m, '-')}");
                    var max = peak.Max() * PeakMultiplier - PeakOffset;

                   // maxHist.Enqueue(max);
                    //if ((smpTotal % 10) == 0)
                    //Write("\r" + new
                    //{
                    //    max = maxHist.Max().ToString("00.00"),
                    //    min = maxHist.Min().ToString("00.00"),
                    //    range = (maxHist.Max() -maxHist.Min()).ToString("00.00"),
                    //    avg = maxHist.Average().ToString("00.00"),
                    //    dev = maxHist.StdDeviation(x => x).ToString("00.00"),

                    //});
                  

                    tiles.Each(t => t.Update(max, max - t.Col , (int) PaintStreamingInterval.TotalMilliseconds));
                    //tiles.Each(t => t.Update(max - t.Col - 0.25 * (2 - Math.Abs(t.Row - 1))));
                    api.UdpStreamer.SendFrame().Wait();
                        


                        
                }
            }
        }
 
    }
}
