using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;


namespace CrazyTank
{
    public class NetClient
    {
        private int udpPort;
        private UdpClient uc; //The udp object that sends the data
        private UdpClient _uc; //The upd object that receives the data
        private Random r = new Random();
        private string ip;
        private Controller ctrl;

        public NetClient(Controller ctrl)
        {
            udpPort = r.Next(9000, 10000);
            _uc = new UdpClient(udpPort);
            uc = new UdpClient();
            this.ctrl = ctrl;
        }

        public bool Connect(string ip, int port)
        {

            {
                this.ip = ip;
                TcpClient client = new TcpClient();
                client.Connect(ip, port);
                Stream ns = client.GetStream();
                BinaryWriter bw = new BinaryWriter(ns);
                bw.Write(udpPort);
                bw.Flush();

                BinaryReader br = new BinaryReader(ns);
                string str = Convert.ToString(br.ReadString());
                readMap(str);
                ns.Close();
                client.Close();

                TankNewMsg msg = new TankNewMsg(ctrl.myTank);
                Send(msg);

                CoronaNewMsg cmsg = new CoronaNewMsg(ctrl.covid);
                Send(cmsg);

                Thread t = new Thread(UDPRecvThread);
                t.IsBackground = true;
                t.Start();
                return true;
            }

        }

        public void Send(Msg msg)
        {
            msg.Send(uc, ip, 7777);
        }

        private void UDPRecvThread()
        {
            byte[] buf = new byte[1024];
            while (true)
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
                buf = _uc.Receive(ref ipep);
                Parse(buf);
            }
        }

        private void Parse(byte[] buf)
        {
            MsgType msgType = MsgType.None;
            Msg msg = null;
            string str = Encoding.UTF32.GetString(buf);
            string[] strs = str.Split('|');
            msgType = (MsgType)Convert.ToInt32(strs[0]);

            switch (msgType)
            {
                case MsgType.TankNew:
                    msg = new TankNewMsg(ctrl);
                    msg.Parse(buf);
                    break;
                case MsgType.TankMove:
                    msg = new TankMoveMsg(ctrl);
                    msg.Parse(buf);
                    break;
                case MsgType.BulletNew:
                    msg = new BulletNewMsg(ctrl);
                    msg.Parse(buf);
                    break;
                case MsgType.CoronaNew:
                    msg = new CoronaNewMsg(ctrl);
                    msg.Parse(buf);
                    break;
                case MsgType.CoronaMove:
                    msg = new CoronaMoveMsg(ctrl);
                    msg.Parse(buf);
                    break;
                case MsgType.HealNew:
                    msg = new OrangeNewMsg(ctrl);
                    msg.Parse(buf);
                    break;
            }
        }

        private void readMap(string str)
        {
            string[] str1 = str.Split('|');
            for (int i = 0; i < str1.Length; i++)
            {
                readMap2(str1[i], i);
            }
        }

        private void readMap2(string str1, int i)
        {
            try
            {
                string[] str2 = str1.Split('n');
                for (int j = 0; j < str2.Length; j++)
                {
                    ctrl.lineMap[i, j] = Convert.ToInt32(str2[j]);
                }
            }
            catch
            {

            }
        }
    }
}
