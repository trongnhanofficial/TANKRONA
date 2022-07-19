using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CrazyTank
{
    public class Explode
    {
        public bool Live { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        private Bitmap bmpExplode = GameImages.explodePic;
        private static int span = 5;
        private int i = 0;

        public static Size size = new Size(28, 28);
        public static Size bmpSize = new Size(28, 28);

        public Explode(int x, int y)
        {
            this.Live = true;
            this.X = x;
            this.Y = y;
        }

        public void Draw(Graphics g)
        {
            if (i == span)
            {
                i = 0;
                Live = false;
                return;
            }
            g.DrawImage(bmpExplode, X, Y, size.Width, size.Width);
            i++;
        }
    }
}
