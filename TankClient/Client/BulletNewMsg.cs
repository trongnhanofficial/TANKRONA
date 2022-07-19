using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CrazyTank
{
    class BulletNewMsg : Msg
    {
        MsgType msgType = MsgType.BulletNew;
        Bullet bullet;
        Controller ctrl;

        /// <summary>
        /// Constructor is used to send packets
        /// </summary>
        /// <param name="bullet"></param>
        public BulletNewMsg(Bullet bullet)
        {
            this.bullet = bullet;
        }

        public BulletNewMsg(Controller ctrl)
        {
            this.ctrl = ctrl;
        }

        public void Send(UdpClient uc, string ip, int udpPort)
        {
            uc.Connect(ip, udpPort);
            //Use "|" in the program to split the content sent
            string str = (int)msgType + "|" + bullet.FromName + "|" + bullet.Id + "|" + bullet.X + "|" + bullet.Y + "|" + (int)bullet.Dir;
            uc.Send(Encoding.UTF32.GetBytes(str), Encoding.UTF32.GetBytes(str).Length);
        }

        public void Parse(byte[] b)
        {
            string str = Encoding.UTF32.GetString(b);
            string[] strs = str.Split('|');
            string fromName = strs[1];
            int id = Convert.ToInt32(strs[2]);

            if (fromName == ctrl.myTank.Name)
            {
                return;
            }
            for (int i = 0; i < ctrl.bullets.Count; i++)
            {
                Bullet tmpB = ctrl.bullets[i];
                if (fromName == tmpB.FromName && id == tmpB.Id)
                {
                    return;
                }
            }
            int x = Convert.ToInt32(strs[3]);
            int y = Convert.ToInt32(strs[4]);
            Direction dir = (Direction)Convert.ToInt32(strs[5]);
            Bullet bullet = new Bullet(fromName, id, x, y, dir);
            ctrl.bullets.Add(bullet);
        }
    }
}
