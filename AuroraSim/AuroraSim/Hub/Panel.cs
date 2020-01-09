using System;
using Microsoft.Owin;
using System.Xml.Linq;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(AuroraSim.Startup))]

namespace AuroraSim
{
    public class Panel
    {
        public Panel(int id, int x, int y, int r)
        {
            Id = id;
            X = x;
            Y = y;
            R = r;
        }

        public int      Id   { get; }
        public int      X    { get; }
        public int      Y    { get; }
        public int      R    { get; }






    }
}
