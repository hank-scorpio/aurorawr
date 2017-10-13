using Net.Things.NanoLeaf;
using System;
using static System.Windows.Media.Colors;
using ImageProcessor;
using ImageProcessor.Imaging;
using System.Drawing;
using Emgu.CV;
using System.Linq;
using System.Diagnostics;

namespace Tests
{


    class CameraBlockStreamer : TileStreamer
    {
        public override int Period => 2500;


        public override void Flushing(AuroraUdpStreamer sender, Layout layout)
        {
            var ms = Stopwatch.StartNew();

            int x = X.Length;
            int y = X.Width;

            using (var img = new ImageFactory().GetCameraFrame())
            {

                var colors = img.BlockifyColors(y, x);
                var tiles = X.ToArray();

                for (int i = 0; i < tiles.Length; i++)
                {
                    var rc = colors[i];
                    var rt = tiles[i];

                    for (int j = 0; j < rt.Length; j++)
                    {
                        try
                        {
                            var c = rc[j];
                            var t = rt[j];
                            t.Color = new TileColor(c.R, c.G, c.B);
                            t.Duration = 5;
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
         
   
            }
            ms.Stop();
                
            Console.WriteLine ($"Updated in {ms.ElapsedMilliseconds}ms");
        }
    }




    public static class ImageFactoryExt
    {
        public static ImageFactory GetCameraFrame(this ImageFactory img)
        {
            var tmp = @"c:\tmp\cam.bmp";
            using (var bmp = new Capture().QueryFrame().Bitmap)
            {
                bmp.Save(tmp);
            }
            return img.Load(tmp);
        }

        public static ImageFactory Blockify(this ImageFactory img, int x, int y)
        {
            var sz = img.Image.Size;
            var h = Math.Max(sz.Width, sz.Height);
            var w = (int)(((double)h) * x / y);
            var px = (int)((double)h / y);

            return img
                .Resize(new ResizeLayer(new Size(w, h), ResizeMode.Stretch))
                .Pixelate(px)
                .Resize(new ResizeLayer(new Size(200 * x, y * 200), ResizeMode.Stretch));
        }
        public static Color[][] BlockifyColors(this ImageFactory img, int x, int y)
        {
            Bitmap bmp = img.Blockify(x, y).Image as Bitmap;

            return Enumerable.Range(0, y).Select(j =>
            Enumerable.Range(0, x).Select(i =>
                bmp.GetPixel(i * 200, j * 200)).ToArray()).ToArray();
        }
    }

}
