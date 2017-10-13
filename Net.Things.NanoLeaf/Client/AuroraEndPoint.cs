using Ext;
using System;

namespace Net.Things.NanoLeaf
{


    public class AuroraEndPoint
    {
        public enum Version { Unknown, Beta, v1 }

        public const ushort     DefaultPort     = 16021;
        public const Version    DefaultVersion  = Version.v1;
      
        public Uri Uri { get; }

        public AuroraEndPoint(string host, ushort port = DefaultPort, Version version = DefaultVersion)
        {
            Uri = new UriBuilder()
            {
                 Scheme = Uri.UriSchemeHttp,
                 Host   = UriExt.GetCanonicalHost(host),
                 Port   = port,
                 Path   = $"api/{version}/".ToLower()
            }
            .Uri;
        }

    }
}
