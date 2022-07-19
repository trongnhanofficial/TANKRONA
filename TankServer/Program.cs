using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;

namespace TankServer
{
    class Program
    {
        public static int tcpPort = 7776;
        public static int udpPort = 7777;
        public int[,] lineMap = new int [40, 30];

        List<Client> clients = new List<Client>();

        public static Random s_Random = new Random(Guid.NewGuid().GetHashCode());

        public void Start()
        {
            try
            {
                MakeMap();
                //udp thread
                Thread t = new Thread(UDPThread);
                t.IsBackground = true;
                t.Start();
                //tcp service 
                Console.WriteLine("TCP port :" + tcpPort);
                TcpListener tcpListener = new TcpListener(IPAddress.Any, tcpPort);
                tcpListener.Start();
                while (true)
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    Stream ns = client.GetStream();
                    BinaryReader br = new BinaryReader(ns);
                    int udpPort = br.ReadInt32(); // read port
                    IPEndPoint rep = (IPEndPoint)client.Client.RemoteEndPoint;
                    Client c = new Client(rep.Address.ToString(), udpPort);
                    clients.Add(c);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("A Client TCP Connect! Addr- " + rep.Address.ToString() + ":" + rep.Port);

                    BinaryWriter bw = new BinaryWriter(ns);
                    bw.Write(WriteMap());
                    bw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error:" + ex.Message);
            }
        }

        static void Main(string[] args)
        {
            new Program().Start();
        }

        private class Client
        {
            public string ip;
            public int udpPort;
            public Client(string ip, int udpPort)
            {
                this.ip = ip;
                this.udpPort = udpPort;
            }
        }


        //Udp packet receive function
        private void UDPThread()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("UDP thread started at port :" + udpPort);
            byte[] buf = new byte[1024];
            UdpClient uc = new UdpClient(udpPort);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                buf = uc.Receive(ref ipep);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("a udp packet received! from " + ipep.Address + ":" + ipep.Port);
                //Forward the received data to each client
                for (int i = 0; i < clients.Count; i++)
                {
                    Client c = clients[i];
                    UdpClient _uc = new UdpClient();
                    _uc.Connect(c.ip, c.udpPort);
                    _uc.Send(buf, buf.Length);
                }
            }
        }

        public int GenerateTile()
        {
            int perCent = s_Random.Next(0, 100);

            if (perCent < 84)
            {
                return 0;
            }
            else if (perCent < 84 + 7)
            {
                return 1;
            }
            else if (perCent < 84 + 7 + 7)
            {
                return 3;
            }
            else if (perCent < 84 + 7 + 7 + 1)
            {
                return 2;
            }
            return 4;
        }

        public void MakeMap()
        {
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    if (i < 8 && j < 6) lineMap[i, j] = 0;
                    else lineMap[i, j] = GenerateTile();
                }
            }
        }

        public string WriteMap()
        {
            string map = "";
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    map += lineMap[i, j] + "n";
                }
                map += "|";
            }
            return map;
        }
    }
}
