using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.AspNet.SignalR.Hubs;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net;

namespace AuroraSim
{
    public class PanelStreamer
    {
     
        public PanelStreamer()
        {
            udp = new UdpClient(55555);
          //  timer = new Timer(FrameReceived, null, 100,100);

            Task.Run(() =>
            {
                while (true)
                {
                    IPEndPoint ep = null;
                    var bytes = udp.Receive(ref ep);
                    PanelsRenderer.Instance.Paint(UnpackFrame(bytes));
                }
                
            });
        }

        readonly UdpClient udp;
     //   readonly Timer timer;
      //  readonly Random rnd = new Random();
        //private void FrameReceived(object state)
        //{
        //    byte[] b = GenFrame().ToArray();

        //    PanelsRenderer.Instance.Paint(UnpackFrame(b));

        //}

        IEnumerable<PanelFrame> UnpackFrame(byte[] b)
        {

            byte n = b[0];
            byte i = 1;
            while (n-- > 0)
            {
                byte id = b[i++];

                for (byte j = b[i++]; j-- > 0; i += 5)
                
                    yield return new PanelFrame(id, b[i], b[i + 1], b[i + 2], b[i + 3], b[i + 4]);
            }
        }

        //IEnumerable<byte> GenFrame()
        //{
        //    yield return (byte)AuroraController.Instance.Layout.Count();
        //    foreach (var p in AuroraController.Instance.Layout)
        //    {
        //        yield return (byte)p.Id;
        //        yield return 1;
        //        yield return (byte)rnd.Next(255);
        //        yield return (byte)rnd.Next(255);
        //        yield return (byte)rnd.Next(255);
        //        yield return 1;
        //        yield return 1;
        //    }

        //}
    }
}