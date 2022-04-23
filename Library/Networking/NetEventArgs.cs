using System.Net;
using System;
using Swordfish.Library.Networking.Interfaces;

namespace Swordfish.Library.Networking
{
    public class NetEventArgs : EventArgs
    {
        public int PacketID;

        public Packet Packet;

        public IPEndPoint EndPoint;

        public NetSession? Session;
    }
}
