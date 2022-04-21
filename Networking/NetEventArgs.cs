using System.Net;
using System;

namespace Swordfish.Networking
{
    public class NetEventArgs : EventArgs
    {
        public Packet Packet;

        public IPEndPoint EndPoint;
    }
}
