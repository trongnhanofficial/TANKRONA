using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CrazyTank
{
    class TankNewMsg : Msg
    {
        private MsgType msgType = MsgType.TankNew;
        private Tank tank;
        private Controller ctrl;

        public TankNewMsg(Tank tank)
        {
            this.tank = tank;
        }

        public TankNewMsg(Controller ctrl)
        {
            this.ctrl = ctrl;
        }

        public void Send(UdpClient uc, string ip, int udpPort)
        {
            uc.Connect(ip, udpPort);

            string str = (int)msgType + "|" + tank.Name + "|" + tank.X + "|" + tank.Y + "|" + (int)tank.Dir 
                + "|" + tank.Color[0] + "|" + tank.Color[1] + "|" + tank.Color[2];
            uc.Send(Encoding.UTF32.GetBytes(str), Encoding.UTF32.GetBytes(str).Length);
        }

        public void Parse(byte[] b)
        {
            string str = Encoding.UTF32.GetString(b);
            string[] strs = str.Split('|');
            string name = strs[1];

            if (name == ctrl.myTank.Name)
            {
                return;
            }
            int x = Convert.ToInt32(strs[2]);
            int y = Convert.ToInt32(strs[3]);
            Direction dir = (Direction)Convert.ToInt32(strs[4]);
            float[] color = new float[3];
            color[0] = Convert.ToSingle(strs[5]);
            color[1] = Convert.ToSingle(strs[6]);
            color[2] = Convert.ToSingle(strs[7]);

            bool exist = false;
            for (int i = 0; i < ctrl.tanks.Count; i++)
            {
                Tank t = ctrl.tanks[i];
                if (t.Name == name)
                {
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                TankNewMsg msg = new TankNewMsg(ctrl.myTank);
                ctrl.nc.Send(msg);
                Tank t = new Tank(name, x, y, dir, color);
                t.Name = name;
                ctrl.tanks.Add(t);
            }
        }
    }
}
