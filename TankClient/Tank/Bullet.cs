using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CrazyTank
{
    public class Bullet
    {
        public string FromName { get; set; }
        public int Id { get; set; }
        public int Life { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Direction Dir { get; set; }
        public int Speed { get; set; }
        public int Level { get; set; }

        public static Size size = new Size(8, 8); //The actual display size
        public static Size bmpSize = new Size(8, 8); //The size of the stock image
        private Bitmap bmpBulletNow;
        private List<Bitmap> bmpBulletAll = GameImages.bulletPic;

        public Bullet(string fromName,int id, int x, int y, Direction dir)
        {
            this.FromName = fromName;
            this.Id = id;
            this.X = x;
            this.Y = y;
            this.Life = 3;/////////////////////////////////
            this.Dir = dir;
            this.Speed = 15;
            this.Level = 1;
        }

        private void Move()
        {
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
            }
        }

        public void Draw(Graphics g)
        {
            switch (Dir)
            {
                case Direction.Up:
                    bmpBulletNow = bmpBulletAll[0];
                    break;
                case Direction.Down:
                    bmpBulletNow = bmpBulletAll[2];
                    break;
                case Direction.Left:
                    bmpBulletNow = bmpBulletAll[3];
                    break;
                case Direction.Right:
                    bmpBulletNow = bmpBulletAll[1];
                    break;
            }
            g.DrawImage(bmpBulletNow, X, Y, size.Width, size.Height);
            Move();
        }

        public Rectangle GetRectangle()
        {
            Rectangle rec = new Rectangle(new Point(X, Y), size);
            return rec;
        }
    }
}
