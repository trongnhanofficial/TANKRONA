using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CrazyTank
{
    class CoronaNewMsg : Msg
    {
        private MsgType msgType = MsgType.CoronaNew;
        private Corona c;
        private Controller ctrl;

        public CoronaNewMsg(Corona c)
        {
            this.c = c;
        }

        public CoronaNewMsg(Controller ctrl)
        {
            this.ctrl = ctrl;
        }


        public void Send(UdpClient uc, string ip, int udpPort)
        {
            uc.Connect(ip, udpPort);

            string str = (int)msgType + "|" + c.Name + "|" + c.X + "|" + c.Y + "|" + (int)c.Dir;    
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

            bool exist = false;
            for (int i = 0; i < ctrl.covids.Count; i++)
            {
                Corona t = ctrl.covids[i];
                if (t.Name == name)
                {
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                CoronaNewMsg msg = new CoronaNewMsg(ctrl.covid);
                ctrl.nc.Send(msg);
                Corona t = new Corona(name, x, y, dir);
                t.Name = name;
                ctrl.covids.Add(t);
            }
        }
    }
}
