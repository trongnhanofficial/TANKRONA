using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace CrazyTank
{
    public class Controller
    {
        public Corona covid;
        public Tank myTank;
        public List<Orange> orange = new List<Orange>();
        public List<Tank> tanks = new List<Tank>();
        public List<Corona> covids = new List<Corona>();
        public List<Bullet> bullets = new List<Bullet>();
        public List<Explode> explodes = new List<Explode>();
        public int[,] lineMap = new int[40, 30];
        public Tile[,] map;

        private Size tis = Tile.size;

        public static Random s_Random = new Random(Guid.NewGuid().GetHashCode());

        private int gameWidth, gameHeight, mapWidth, mapHeight;
        private bool dirUp, cDirUp;
        private bool dirDown, cDirDown;
        private bool dirLeft, cDirLeft;
        private bool dirRight, cDirRight;
        private Direction dirOld, cDirOld;
        public NetClient nc;

        public Controller(int gameWidth, int gameHeight)
        {
            this.gameWidth = gameWidth;
            this.gameHeight = gameHeight;
            this.mapWidth = gameWidth / tis.Width;
            this.mapHeight = gameHeight / tis.Height;
            map = new Tile[mapWidth, mapHeight];
        }

        private void LoadMap()
        {
            TileType tmpTileType;
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    tmpTileType = (TileType)lineMap[i, j];
                    if (tmpTileType != TileType.None)
                    {
                        map[i, j] = new Tile(i * tis.Width, j * tis.Width, tmpTileType);
                    }
                }
            }
        }

        public bool NetClientConnect(string ip, int port, float[] tankColor, string name)
        {
            Random r = new Random();
            int rNum = r.Next(100, 999);
            myTank = new Tank(name + rNum.ToString(), s_Random.Next(5, gameWidth), s_Random.Next(5, gameHeight), Direction.Up, tankColor);

            covid = new Corona("corona" + rNum.ToString(), s_Random.Next(15, gameWidth), s_Random.Next(15, gameHeight), Direction.Right);

            nc = new NetClient(this);
            if (nc.Connect(ip, port))
            {
                LoadMap();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Paint(Graphics g)
        {
            if (myTank != null)
            {
                g.DrawString(myTank.Name + " Life:  " + myTank.Life, new Font("Arial", 8), new SolidBrush(Color.White), 20, 20);
                g.DrawString("EnemyTanks  Count: " + tanks.Count, new Font("Arial", 8), new SolidBrush(Color.White), 20, 50);
            }
            //bullet
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].Life > 0)
                {
                    bullets[i].Draw(g);
                    BulletMove(bullets[i]);
                }
            }
            //orange
            if (orange.Count > 0)
            {
                for (int i = 0; i < orange.Count; i++)
                {
                    orange[i].Draw(g);
                }
            }
            //opponents's tanks
            for (int i = 0; i < tanks.Count; i++)
            {
                if (tanks[i].Life > 0)
                {
                    tanks[i].Draw(g);
                    tanks[i].Move();
                }
            }
            //covids
            for (int i = 0; i < covids.Count; i++)
            {
                if (covids[i].Life > 0)
                {
                    covids[i].Draw(g);
                    covids[i].Move();
                }
            }

            if (covid != null && covid.Life > 0)
            {
                covid.Draw(g);
                CoronaMove(covid);
            }

            //my tank
            if (myTank != null && myTank.Life > 0)
            {
                myTank.Draw(g);
                TankMove(myTank);
            }
            //Blocks
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    if (map[i, j] != null)
                    {
                        map[i, j].Draw(g);
                    }
                }
            }
            //Explosion effect
            for (int i = 0; i < explodes.Count; i++)
            {
                if (explodes[i].Live == true)
                {
                    explodes[i].Draw(g);
                }
                else
                {
                    explodes.Remove(explodes[i]);
                    i--;
                }
            }
        }

        private void TankMove(Tank tank)
        {
            dirOld = tank.Dir;
            if (tank.Dir != Direction.Stop)
            {
                tank.DirOld = tank.Dir;
            }

            if (dirUp)
            {
                tank.Dir = Direction.Up;
                tank.Move();
            }
            if (dirDown)
            {
                tank.Dir = Direction.Down;
                tank.Move();
            }
            if (dirLeft)
            {
                tank.Dir = Direction.Left;
                tank.Move();
            }
            if (dirRight)
            {
                tank.Dir = Direction.Right;
                tank.Move();
            }
            if (!dirUp && !dirDown && !dirLeft && !dirRight)
            {
                tank.Dir = Direction.Stop;
            }
            //Collision detection with boundaries
            if (tank.X < 0 || tank.Y < 0 || tank.X > gameWidth - Tank.size.Width || tank.Y > gameHeight - Tank.size.Height)
            {
                myTank.X = myTank.XOld;
                myTank.Y = myTank.YOld;
                tank.Dir = Direction.Stop;
            }
            //Collision detection with tanks
            for (int i = 0; i < tanks.Count; i++)
            {
                if (myTank != null && CollisionDetection(tanks[i].GetRectangle(), myTank.GetRectangle()))
                {
                    myTank.X = myTank.XOld;
                    myTank.Y = myTank.YOld;
                    tank.Dir = Direction.Stop;
                }
            }
            //collision detection with covids
            for (int i = 0; i < covids.Count; i++)
            {
                if (myTank != null && CollisionDetection(covids[i].GetRectangle(), myTank.GetRectangle()))
                {
                    myTank.X = myTank.XOld;
                    myTank.Y = myTank.YOld;
                    tank.Dir = Direction.Stop;
                    Explode e = new Explode(covids[i].X, covids[i].Y);
                    explodes.Add(e);
                    if (myTank.Life > 0) myTank.Life -= 1;
                }
            }
            if (myTank != null && CollisionDetection(covid.GetRectangle(), myTank.GetRectangle()))
            {
                myTank.X = myTank.XOld;
                myTank.Y = myTank.YOld;
                tank.Dir = Direction.Stop;
                Explode e = new Explode(covid.X, covid.Y);
                explodes.Add(e);
                if (myTank.Life > 0) myTank.Life -= 1;
            }
            //Collision with orange
            for (int i = 0; i < orange.Count; i++)
            {
                if (myTank != null && CollisionDetection(orange[i].GetRectangle(), myTank.GetRectangle()))
                {
                    tank.Dir = Direction.Stop;
                    if (myTank.Life < 3) myTank.Life += 1;
                    orange.Remove(orange[i]);
                }
            }
            for (int i = 0; i < tanks.Count; i++)
            {
                for (int j = 0; j < orange.Count; j++) {
                    if (tanks[i] != null && CollisionDetection(tanks[i].GetRectangle(), orange[j].GetRectangle()))
                    {
                        tanks[i].Dir = Direction.Stop;
                        if (tanks[i].Life < 3) tanks[i].Life += 1;
                        orange.Remove(orange[j]);
                    }
                }
            }
            //Collision detection with map blocks
            int tmpX = myTank.X / tis.Width;
            int tmpY = myTank.Y / tis.Height;
            for (int i = tmpX - 1; i < tmpX + 3; i++)
            {
                for (int j = tmpY - 1; j < tmpY + 3; j++)
                {
                    if (i < 0 || j < 0 || i >= mapWidth || j >= mapHeight)
                    {
                        continue;
                    }
                    if (map[i, j] != null)
                    {
                        if (CollisionDetection(map[i, j].GetRectangle(), myTank.GetRectangle()))
                        {
                            myTank.X = myTank.XOld;
                            myTank.Y = myTank.YOld;
                            tank.Dir = Direction.Stop;
                        }
                    }
                }
            }
            //message send when the direction is changed
            if (nc != null && tank.Dir != dirOld)
            {
                TankMoveMsg msg = new TankMoveMsg(tank.Name, tank.X, tank.Y, tank.Dir);
                nc.Send(msg);
            }
        }

        private void CoronaMove(Corona c)
        {
            cDirOld = c.Dir;
            if (c.Dir != Direction.Stop)
            {
                c.DirOld = c.Dir;
            }

            if (cDirUp)
            {
                c.Dir = Direction.Up;
                c.Move();
            }
            if (cDirDown)
            {
                c.Dir = Direction.Down;
                c.Move();
            }
            if (cDirLeft)
            {
                c.Dir = Direction.Left;
                c.Move();
            }
            if (cDirRight)
            {
                c.Dir = Direction.Right;
                c.Move();
            }
            if (!cDirUp && !cDirDown && !cDirLeft && !cDirRight)
            {
                c.Dir = Direction.Stop;
            }
            //Collision detection with boundaries
            if (c.X < 0 || c.Y < 0 || c.X > gameWidth - Tank.size.Width || c.Y > gameHeight - Tank.size.Height)
            {
                c.X = c.XOld;
                c.Y = c.YOld;
                c.Dir = Direction.Stop;
            }
            //Collision detection with tanks
            for (int i = 0; i < tanks.Count; i++)
            {
                if (covid != null && CollisionDetection(tanks[i].GetRectangle(), covid.GetRectangle()))
                {
                    covid.X = covid.XOld;
                    covid.Y = covid.YOld;
                    c.Dir = Direction.Stop;
                    Explode e = new Explode(tanks[i].X, tanks[i].Y);
                    explodes.Add(e);

                    if (tanks[i].Life > 0) tanks[i].Life -= 1;
                    else tanks.Remove(tanks[i]);
                }
            }
            if (covid != null && CollisionDetection(myTank.GetRectangle(), covid.GetRectangle()))
            {
                covid.X = covid.XOld;
                covid.Y = covid.YOld;
                c.Dir = Direction.Stop;

                Explode e = new Explode(myTank.X, myTank.Y);
                explodes.Add(e);
                if (myTank.Life > 0) myTank.Life -= 1;
            }
            //Collision detection with map blocks
            int tmpX = covid.X / tis.Width;
            int tmpY = covid.Y / tis.Height;
            for (int i = tmpX - 1; i < tmpX + 3; i++)
            {
                for (int j = tmpY - 1; j < tmpY + 3; j++)
                {
                    if (i < 0 || j < 0 || i >= mapWidth || j >= mapHeight)
                    {
                        continue;
                    }
                    if (map[i, j] != null)
                    {
                        if (CollisionDetection(map[i, j].GetRectangle(), covid.GetRectangle()))
                        {
                            covid.X = covid.XOld;
                            covid.Y = covid.YOld;
                            covid.Dir = Direction.Stop;
                        }
                    }
                }
            }
            //message send when the direction is changed
            if (nc != null && c.Dir != cDirOld)
            {
                CoronaMoveMsg msg = new CoronaMoveMsg(c.Name, c.X, c.Y, c.Dir);
                nc.Send(msg);
            }
        }

        public void BulletMove(Bullet bullet)
        {
            //Bullets collide with tanks
            for (int i = 0; i < tanks.Count; i++)
            {
                if (CollisionDetection(tanks[i].GetRectangle(), bullet.GetRectangle())
                    && tanks[i].Name != bullet.FromName)
                {
                    Explode e = new Explode(tanks[i].X, tanks[i].Y);
                    explodes.Add(e);

                    tanks[i].Life -= 1;
                    if (tanks[i].Life == 0) tanks.Remove(tanks[i]);
                    bullets.Remove(bullet);
                    return;
                }
            }
            if (myTank != null && CollisionDetection(myTank.GetRectangle(), bullet.GetRectangle())
                && myTank.Name != bullet.FromName && myTank.Life > 0)
            {
                Explode e = new Explode(myTank.X, myTank.Y);
                explodes.Add(e);

                myTank.Life -= 1;
                bullets.Remove(bullet);
                return;
            }
            //bullets collide with covids
            for (int i = 0; i < covids.Count; i++)
            {
                if (CollisionDetection(covids[i].GetRectangle(), bullet.GetRectangle())
                    && covids[i].Name != bullet.FromName)
                {
                    Explode e = new Explode(covids[i].X, covids[i].Y);
                    explodes.Add(e);

                    covids[i].Life -= 1;
                    if (covids[i].Life == 0) 
                    {
                        Orange o = new Orange(s_Random.ToString(), covids[i].X, covids[i].Y);
                        orange.Add(o);
                        covids.Remove(covids[i]); 
                    }
                    bullets.Remove(bullet);
                    return;
                }
            }
            if (covid != null && CollisionDetection(covid.GetRectangle(), bullet.GetRectangle())
                && covid.Name != bullet.FromName && covid.Life > 0)
            {
                Explode e = new Explode(covid.X, covid.Y);
                explodes.Add(e);

                covid.Life -= 1;
                if (covid.Life == 0)
                {
                    Orange o = new Orange(s_Random.ToString(), covid.X, covid.Y);
                    orange.Add(o);

                    Random r = new Random();
                    int rNum = r.Next(100, 999);
                    covid = new Corona("corona" + rNum.ToString(), s_Random.Next(0, gameWidth), s_Random.Next(0, gameHeight), Direction.Up);
                    CoronaNewMsg msg = new CoronaNewMsg(covid);
                    nc.Send(msg);
                }
                bullets.Remove(bullet);
                return;
            }

            //Bullets collide with bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                if (CollisionDetection(bullets[i].GetRectangle(), bullet.GetRectangle()) && bullet.FromName != bullets[i].FromName)
                {
                    bullets[i].Life = 0;
                    bullets.Remove(bullets[i]);
                    bullet.Life = 0;
                    bullets.Remove(bullet);
                    return;
                }
            }

            //Bullets collide with blocks
            int tmpX = bullet.X / tis.Width;
            int tmpY = bullet.Y / tis.Height;
            for (int i = tmpX - 1; i < tmpX + 2; i++)
            {
                for (int j = tmpY - 1; j < tmpY + 2; j++)
                {
                    if (i < 0 || j < 0 || i >= mapWidth || j >= mapHeight)
                    {
                        continue;
                    }
                    if (map[i, j] != null)
                    {
                        if (map[i, j].Type == TileType.Brick || map[i, j].Type == TileType.Iron)
                        {
                            if (CollisionDetection(map[i, j].GetRectangle(), bullet.GetRectangle()))
                            {
                                if(map[i, j].Type == TileType.Brick) map[i, j] = null;

                                Explode e = new Explode(bullet.X - Explode.size.Width / 2 + Bullet.size.Width / 2,
                                    bullet.Y - Explode.size.Height / 2 + Bullet.size.Height / 2);
                                explodes.Add(e);

                                bullet.Life = 0;
                                bullets.Remove(bullet);
                                return;
                            }
                        }
                    }
                }
            }

            //Bullets flew out of the border
            if (bullet.X < 0 || bullet.Y < 0
                || bullet.X > gameWidth - Bullet.size.Width
                || bullet.Y > gameHeight - Bullet.size.Height)
            {
                bullet.Life = 0;
                bullets.Remove(bullet);
            }
        }

        public void KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.W || e.KeyChar == (char)Keys.W + 32)
            {
                dirUp = true;
                dirDown = false;
                dirLeft = false;
                dirRight = false;
            }
            if (e.KeyChar == (char)Keys.A || e.KeyChar == (char)Keys.A + 32)
            {
                dirUp = false;
                dirDown = false;
                dirLeft = true;
                dirRight = false;
            }
            if (e.KeyChar == (char)Keys.S || e.KeyChar == (char)Keys.S + 32)
            {
                dirUp = false;
                dirDown = true;
                dirLeft = false;
                dirRight = false;
            }
            if (e.KeyChar == (char)Keys.D || e.KeyChar == (char)Keys.D + 32)
            {
                dirUp = false;
                dirDown = false;
                dirLeft = false;
                dirRight = true;
            }
        }

        public void KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A || e.KeyCode == (Keys)((char)Keys.A + 32))
            {
                dirLeft = false;
            }
            if (e.KeyCode == Keys.D || e.KeyCode == (Keys)((char)Keys.D + 32))
            {
                dirRight = false;
            }
            if (e.KeyCode == Keys.W || e.KeyCode == (Keys)((char)Keys.W + 32))
            {
                dirUp = false;
            }
            if (e.KeyCode == Keys.S || e.KeyCode == (Keys)((char)Keys.S + 32))
            {
                dirDown = false;
            }

            if (e.KeyCode == Keys.Space && nc != null)
            {
                if (myTank.Life > 0)
                {
                    Bullet b = myTank.Fire();
                    bullets.Add(b);
                    BulletNewMsg msg = new BulletNewMsg(b);
                    nc.Send(msg);
                }
            }
        }

        public void CoronaMove2(object sender, EventArgs e)
        {
            int x = s_Random.Next(1, 4);
            if (x == 1)
            {
                cDirUp = true;
                cDirDown = false;
                cDirLeft = false;
                cDirRight = false;
            }
            if (x == 2)
            {
                cDirUp = false;
                cDirDown = false;
                cDirLeft = true;
                cDirRight = false;
            }
            if (x == 3)
            {
                cDirUp = false;
                cDirDown = true;
                cDirLeft = false;
                cDirRight = false;
            }
            if (x == 4)
            {
                cDirUp = false;
                cDirDown = false;
                cDirLeft = false;
                cDirRight = true;
            }
        }

        public void coronaShoot(object sender, EventArgs e)
        {
            if (covid.Life > 0)
            {
                Bullet b = covid.Fire();
                bullets.Add(b);
                BulletNewMsg msg = new BulletNewMsg(b);
                nc.Send(msg);
            }
        }

        private bool CollisionDetection(Rectangle rec1, Rectangle rec2)
        {
            if ((rec1.Width == 0 && rec1.Height == 0) || (rec2.Width == 0 && rec2.Height == 0))
            {
                return false;
            }
            if (rec1.X + rec1.Width > rec2.X && rec1.Y + rec1.Height > rec2.Y
                && rec1.X < rec2.X + rec2.Width && rec1.Y < rec2.Y + rec2.Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
