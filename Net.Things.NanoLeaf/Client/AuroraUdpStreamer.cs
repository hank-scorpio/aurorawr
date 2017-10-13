using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ext;

namespace Net.Things.NanoLeaf
{
    public class AuroraUdpStreamer : UdpClient
    {
        const int DefaultPort = 60221;

        public int Port { get; set; } 
            = DefaultPort;

        public AuroraClient AuroraClient { get; }


        public delegate void FlushingStreamEventHandler(AuroraUdpStreamer sender, Layout layout);

        public event FlushingStreamEventHandler Flushing;
        public event FlushingStreamEventHandler Starting;
        public event FlushingStreamEventHandler Stopped;

        public AuroraUdpStreamer(AuroraClient client)
        {
            AuroraClient = client;
        }

        public async Task SendFrame()
        {
            var tiles = AuroraClient.Layout.Tiles;

            Flushing?.Invoke(this, AuroraClient.Layout);

            Tile[] changes = tiles.Where(x => x.HasChanges).ToArray();
            if (changes.Length == 0) return;

            byte[] bytes = GetFrameBytes(changes);
            Console.WriteLine($"Streaming {bytes.Length} bytes ({changes.Length} panels)");
            await SendAsync(bytes, bytes.Length, AuroraClient.EndPoint.Uri.Host, Port);

            foreach (Tile tile in changes) tile.HasChanges = false;
        }

        public static byte[] GetFrameBytes(Tile[] tiles)
        {
            var bytes = new List<byte> { (byte)tiles.Length };
            foreach (Tile t in tiles)
            {
                bytes.Add((byte)t.Id);
                bytes.Add((byte)t.Frames.Count());

                foreach (var f in t.Frames)
                {
                    bytes.AddRange(Struct.GetBytes(f));
                }
            }
            return bytes.ToArray();
        }




        Timer flushTimer;
        Timer FlushTimer 
            => flushTimer ?? (flushTimer = new Timer(_ => SendFrame()));

        public bool IsStreaming { get; protected set; }
        
        public async Task Run<T>() where T : TileStreamer, new()
        {
            var streamer = new T();

            Flushing += streamer.Flushing;
            Starting += streamer.Starting;
            Stopped += streamer.Starting;

            streamer.Starting(this, AuroraClient.Layout);
            StartStreaming(streamer.Period);
            await WaitWhileStreaming();

            Flushing -= streamer.Flushing;
            Starting -= streamer.Starting;
            Stopped -= streamer.Starting;
        }

        public async Task WaitWhileStreaming()
            => await Task.Run(() => { while (IsStreaming) Thread.Sleep(500); });

        public bool StartStreaming(int period = 50)
            => IsStreaming = FlushTimer.Change(0, period);

        public bool StopStreaming()
            => IsStreaming = !FlushTimer.Change(Timeout.Infinite, Timeout.Infinite);


    }
}
