using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CrazyTank
{
    public enum MsgType
    {
        None,TankNew,TankMove,BulletNew, CoronaNew, CoronaMove, HealNew
    }

    public interface Msg
    {
        /// <summary>
        /// Message sending
        /// </summary>
        /// <param name="uc">udp the object to which the packet is sent</param>
        /// <param name="ip">server IP address</param>
        /// <param name="udpPort">upd port of the server</param>
        void Send(UdpClient uc, string ip, int udpPort);

        /// <summary>
        /// Message parsing
        /// </summary>
        /// <param name="b">byte array to parse</param>
        void Parse(byte[] b);
    }
}
