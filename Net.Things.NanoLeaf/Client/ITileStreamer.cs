namespace Net.Things.NanoLeaf
{
    public abstract class TileStreamer
    {

        public virtual int Period => 50;

        Layout Layout;

        protected TileGrid Grid => Layout.Grid;
        protected TileRank X => Grid.X;
        protected TileRank Y => Grid.Y;

        public abstract void Flushing(AuroraUdpStreamer sender, Layout layout);
     
        public virtual void Starting(AuroraUdpStreamer sender, Layout layout)
        {
            Layout = layout;
        }
        public virtual void Stopped(AuroraUdpStreamer sender, Layout layout)
        {
            Layout = null;
        }
    }
}