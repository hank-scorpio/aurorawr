using System.Collections.Generic;

namespace Net.Things.NanoLeaf
{
    public static class Aurora
    {
        public static AuroraClient CreateClient(AuroraEndPoint ep)
            => new AuroraClient(ep);
        public static AuroraClient CreateClient(string host, ushort port = AuroraEndPoint.DefaultPort)
           => CreateClient(new AuroraEndPoint(host, port));

        public static AuroraClient Leaf1 = Aurora
            .CreateClient("leaf1.home")
            .WithAuthToken("9LXgJTxQRdDWwOsgShlxfBYP5s0retal");


        public static IEnumerable<AuroraClient> Discover()
            => new[] { Leaf1 };
    }
}
