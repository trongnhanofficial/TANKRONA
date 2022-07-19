using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CrazyTank
{
    class OrangeNewMsg : Msg
    {
        MsgType msgType = MsgType.HealNew;
        Orange orange;
        Controller ctrl;

        /// <summary>
        /// Constructor is used to send packets
        /// </summary>
        /// <param name="bullet"></param>
        public OrangeNewMsg(Orange o)
        {
            this.orange = o;
        }

        public OrangeNewMsg(Controller ctrl)
        {
            this.ctrl = ctrl;
        }

        public void Send(UdpClient uc, string ip, int udpPort)
        {
            uc.Connect(ip, udpPort);
            //Use "|" in the program to split the content sent
            string str = (int)msgType + "|" + orange.Name + "|" + orange.X + "|" + orange.Y;
            uc.Send(Encoding.UTF32.GetBytes(str), Encoding.UTF32.GetBytes(str).Length);
        }

        public void Parse(byte[] b)
        {
            string str = Encoding.UTF32.GetString(b);
            string[] strs = str.Split('|');
            string Name = strs[1];
            for (int i = 0; i < ctrl.orange.Count; i++)
            {
                Orange tmpB = ctrl.orange[i];
                if (Name == tmpB.Name)
                {
                    return;
                }
            }

            int x = Convert.ToInt32(strs[2]);
            int y = Convert.ToInt32(strs[3]);

            Orange o = new Orange(Name, x, y);
            ctrl.orange.Add(orange);
        }
    }
}