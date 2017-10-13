using Net.Things.NanoLeaf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{

    class ApiTests
    {
        static void Main()
        {
            var client = Aurora
                .CreateClient("leaf1.home")
                .WithAuthToken("9LXgJTxQRdDWwOsgShlxfBYP5s0retal");

            RunBasicTests(client);

            RunStream<CameraBlockStreamer>(client).Wait();


            End();
        }

        async static Task RunStream<T>(AuroraClient client) where T: TileStreamer, new()
        {
            Console.WriteLine($"* Run tile streamer: {typeof(T).Name}");
            await client.UdpStreamer.Run<T>();
        }

        static void End()
        {
            Console.WriteLine();
            Console.Write("--END--");
            Console.ReadLine();
        }

        static void RunBasicTests(AuroraClient client)
        {
            GetLayout();
            GetState();
            SetBrightness(20);
            SetBrightness(50);
            GetState();

            void GetLayout()
            {
                Console.WriteLine("* Get layout");
                foreach (var t in client.Layout.Tiles) Console.WriteLine(t);
                Console.WriteLine();
            }
            void GetState()
            {
                Console.WriteLine("* Get state");
                var state = client.GetState().Result;
                Console.WriteLine($"Brightness={state.Brightness}, ColorMode={state.ColorMode} On={state.On}");
                Console.WriteLine();
            };
            void SetBrightness(int level)
            {
                Console.WriteLine($"* Set Brightness to {level}");
                client.Brightness = level;
                Console.WriteLine();
                Thread.Sleep(500);
            };
        }
    }
}
