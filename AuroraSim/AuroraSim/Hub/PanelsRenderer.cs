using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;

namespace AuroraSim
{
    public class PanelsRenderer : IPanelsRenderer
    {
        readonly static Lazy<PanelsRenderer> instance 
            = new Lazy<PanelsRenderer>();

        public static IPanelsRenderer Instance 
            => instance.Value;
        
        readonly IPanelsRenderer hub 
            = GlobalHost.ConnectionManager
                .GetHubContext<PanelsRendererHub, IPanelsRenderer>()
                .Clients.All;
        public void Paint(IEnumerable<PanelFrame> frames) 
            => hub.Paint(frames);
    }

  



}