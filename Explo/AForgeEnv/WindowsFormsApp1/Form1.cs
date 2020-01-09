using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Controls;
using AForge.Imaging;
using static System.Drawing.Color;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {



            Bitmap src = Bitmap.FromFile(@"C:\Users\phil\Pictures\vlcsnap-2017-10-01-21h39m20s991.png") as Bitmap;
            var stats = new ImageStatistics(src);

            histogram1.Color = Color.FromArgb(60, Color.Red);
            histogram1.Values = stats.Red.Values; ;

            //typeof(ImageStatistics).GetMembers().Where(x => x.MemberType == typeof(Histogram))

            //foreach (var ch in new[] 
            //{
            //    (Red,       stats.Red),
            //    (Blue,      stats.Blue),
            //    (Green,     stats.Green),
            //    (Green,     stats.),
            //})

            var c = new Histogram();
                
                c.Location = histogram1.Location;
                c.Size = histogram1.Size;
                c.Color = Color.FromArgb(60, Color.Red);
                c.Values = stats.Red.Values;
        }
    }
}
