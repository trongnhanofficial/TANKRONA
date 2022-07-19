using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CrazyTank
{
    public class Corona
    {
        public static Random s_Random = new Random(Guid.NewGuid().GetHashCode());

        public string Name { get; set; }
        public int Life { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int XOld { get; set; }
        public int YOld { get; set; }
        public Direction Dir { get; set; }
        public Direction DirOld { get; set; }
        public int Speed { get; set; }
        public int Level { get; set; }

        public static Size size = new Size(28, 28); //actual display size 
        public static Size bmpSize = new Size(28, 28); //size of image.
        private Bitmap bmpCoronaNow = GameImages.coronaPic;
        //private List<Bitmap>[] bmpCoronaAll = new List<Bitmap>[1];
        private int bulletId = 0;

        public Corona(string name, int x, int y, Direction dir)
        {
            this.Name = name;
            this.X = x;
            this.Y = y;
            this.Life = s_Random.Next(1, 5);
            this.Dir = dir;
            this.DirOld = dir;
            this.Speed = s_Random.Next(1,5);
            this.Level = 0;
        }

        public void Move()
        {
            this.XOld = this.X;
            this.YOld = this.Y;
            switch (Dir)
            {
                case Direction.Up:
                    Y -= Speed;
                    break;
                case Direction.Down:
                    Y += Speed;
                    break;
                case Direction.Left:
                    X -= Speed;
                    break;
                case Direction.Right:
                    X += Speed;
                    break;
                case Direction.Stop:
                    break;
            }
        }

        public Bullet Fire()
        {
            //bullet move by the recent direction move if stage is stop
            Direction bulletDir = this.Dir;
            if (this.Dir == Direction.Stop)
            {
                bulletDir = this.DirOld;
            }
            Bullet bullet = new Bullet(Name, bulletId++, X + size.Width / 2 - Bullet.size.Width / 2,
                Y + size.Height / 2 - Bullet.size.Height / 2, bulletDir);
            return bullet;
        }

        public void Draw(Graphics g)
        {
            //bmpCoronaNow = bmpCoronaAll[Level][0];

            g.DrawImage(bmpCoronaNow, X, Y, size.Width, size.Height);
        }

        public Rectangle GetRectangle()
        {
            Rectangle rec = new Rectangle(new Point(X, Y), size);
            return rec;
        }
    }
}
