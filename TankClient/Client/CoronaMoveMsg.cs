using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CrazyTank
{
    class CoronaMoveMsg : Msg
    {
        private MsgType msgType = MsgType.CoronaMove;
        private string name;
        private int x, y;
        private Direction dir;
        private Controller ctrl;

        public CoronaMoveMsg(string name, int x, int y, Direction dir)
        {
            this.name = name;
            this.dir = dir;
            this.x = x;
            this.y = y;
        }

        public CoronaMoveMsg(Controller ctrl)
        {
            this.ctrl = ctrl;
        }


        public void Send(UdpClient uc, string ip, int udpPort)
        {
            uc.Connect(ip, udpPort);
            
            string str = (int)msgType + "|" + name + "|" + x + "|" + y + "|" + (int)dir;
            uc.Send(Encoding.UTF32.GetBytes(str), Encoding.UTF32.GetBytes(str).Length);
        }

        public void Parse(byte[] b)
        {
            string str = Encoding.UTF32.GetString(b);
            string[] strs = str.Split('|');
            string name = strs[1];

            if (name == ctrl.covid.Name)
            {
                return;
            }
            int x = Convert.ToInt32(strs[2]);
            int y = Convert.ToInt32(strs[3]);
            Direction dir = (Direction)Convert.ToInt32(strs[4]);
            for (int i = 0; i < ctrl.covids.Count; i++)
            {
                Corona t = ctrl.covids[i];
                if (t.Name == name)
                {
                    t.Dir = dir;
                    t.X = x;
                    t.Y = y;
                    break;
                }
            }
        }
    }
}
