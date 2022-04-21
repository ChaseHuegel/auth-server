using System.Net;
using System;

namespace Swordfish.Networking
{
    public struct ClientConnection
    {
        public IPEndPoint EndPoint { get; set; }

        public int ID { get; set; }
    }
}
