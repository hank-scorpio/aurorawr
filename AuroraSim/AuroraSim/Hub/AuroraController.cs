using System;
using System.Collections.Generic;
using System.Linq;

namespace AuroraSim
{
    public class AuroraController
    {
        const string LayoutString = "10 150 1 100 100 60 2 324 56 0 3 249 -159 60 4 174 56 240 5 324 -29 60 6 -49 100 60 7 399 99 300 8 174 -29 60 9 25 56 120 10 249 -73 240";
        readonly static Lazy<AuroraController> instance = new Lazy<AuroraController>();
        public static AuroraController Instance => instance.Value;

        public PanelStreamer streamer;
        public IEnumerable<Panel> Layout => layout;

        readonly IReadOnlyCollection<Panel> layout;
        public AuroraController()
        {
            layout = UnpackLayout(LayoutString)
                .ToArray();
            streamer = new PanelStreamer();


        }

        static IEnumerable<Panel> UnpackLayout(string layout)
        {
            var data = layout.Split(' ').Select(int.Parse).ToArray();
            for (int i = 2; i < data.Length; i += 4)
            {
                yield return new Panel(data[i], data[i + 1], data[i + 2], data[i + 3]);
            }

        }
    }
}